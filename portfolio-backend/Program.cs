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

// Adicionar configura��o do Swagger
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Usar configura��o do Swagger apenas em desenvolvimento
// if (app.Environment.IsDevelopment())
// {
    app.UseSwaggerConfiguration();
// }

app.UseAuthentication();
app.UseAuthorization();
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
