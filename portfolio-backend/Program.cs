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

// Carregar configurações do appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var googleAuthSettings = builder.Configuration.GetSection("GoogleAuth").Get<GoogleAuthSettings>();
if (jwtSettings == null || googleAuthSettings == null)
{
    throw new Exception("Configurações ausentes em appsettings.json!");
}

// Configurar autenticação com JWT
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

// Tornar configurações disponíveis via injeção de dependência
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("GoogleAuth"));

// Adicionar CORS - Política para permitir qualquer origem, método e cabeçalho
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

//// Adicionar página de exceção para desenvolvimento
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

// Ativar autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Mapear controllers
app.MapControllers();

app.Run();

// Definição das classes de configuração
public class JwtSettings
{
    public string? Secret { get; set; }
}

public class GoogleAuthSettings
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}
