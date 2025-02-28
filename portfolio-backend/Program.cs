using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adicionando suporte a controllers
builder.Services.AddControllers();


// Adicionando suporte para Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Portfolio Backend Guilherme",
        Version = "v1"
    });
});

// Carregar configura��es do appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var googleAuthSettings = builder.Configuration.GetSection("GoogleAuth").Get<GoogleAuthSettings>();
if (jwtSettings == null || googleAuthSettings == null)
{
    throw new Exception("Configura��es ausentes em appsettings.json!");
}

// Configurar autentica��o com JWT
var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();

// Tornar configura��es dispon�veis via inje��o de depend�ncia
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuth"));

// Adicionar CORS - Pol�tica para permitir qualquer origem, m�todo e cabe�alho
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

//// Adicionar p�gina de exce��o para desenvolvimento
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio Backend Guilherme v1");

    // Permitir o Swagger funcionar em HTTP e HTTPS
    if (app.Environment.IsDevelopment())
    {
        c.SwaggerEndpoint("http://localhost:5281/swagger/v1/swagger.json", "HTTP - Portfolio Backend");
        c.SwaggerEndpoint("https://localhost:7199/swagger/v1/swagger.json", "HTTPS - Portfolio Backend");
    }

    c.RoutePrefix = "swagger"; // Evita que o Swagger tente carregar diretamente na raiz
});


// Ativar CORS
app.UseCors("AllowAll");

// Ativar autentica��o e autoriza��o
app.UseAuthentication();
app.UseAuthorization();

// Mapear controllers
app.MapControllers();

app.Run();

// Defini��o das classes de configura��o
public class JwtSettings
{
    public string? Secret { get; set; }
}

public class GoogleAuthSettings
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}
