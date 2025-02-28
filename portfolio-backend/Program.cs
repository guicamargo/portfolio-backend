using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

// Adicionar configuração do Swagger
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Usar configuração do Swagger apenas em desenvolvimento
// if (app.Environment.IsDevelopment())
// {
    app.UseSwaggerConfiguration();
// }

app.UseAuthentication();
app.UseAuthorization();
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
