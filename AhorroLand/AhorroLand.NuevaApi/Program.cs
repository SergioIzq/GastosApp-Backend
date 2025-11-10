using AhorroLand.Application;
using AhorroLand.Infrastructure;
using AhorroLand.Infrastructure.Configuration;
using AhorroLand.Infrastructure.TypesHandlers;
using AhorroLand.Middleware;
using AhorroLand.Shared.Application;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ?? OPTIMIZACIÓN: Configurar Kestrel para HTTP/2 y keep-alive
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 1000;
    options.Limits.MaxConcurrentUpgradedConnections = 1000;
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);

    // Habilitar HTTP/2
    options.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // ?? OPTIMIZACIÓN: Configurar System.Text.Json para mejor rendimiento
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Sin conversión de nombres
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Definir el esquema de seguridad JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el formato: Bearer {token}"
    });

    // Requerir el esquema de seguridad globalmente
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
        new OpenApiSecurityScheme
            {
  Reference = new OpenApiReference
           {
         Type = ReferenceType.SecurityScheme,
       Id = "Bearer"
   }
       },
            Array.Empty<string>()
        }
    });
});

// ?? OPTIMIZACIÓN: Response Compression (Brotli + Gzip)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

DefaultTypeMap.MatchNamesWithUnderscores = true;

DapperTypeHandlerRegistration.RegisterGuidValueObjectHandlers();

MapsterConfig.RegisterMapsterConfiguration(builder.Services);

builder.Services.AddApplication();
builder.Services.AddSharedApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ?? Configuración de autenticación JWT
var jwtKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey no está configurada.");
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "AhorroLand";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "AhorroLand";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAhorroLandExceptionHandling();

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
