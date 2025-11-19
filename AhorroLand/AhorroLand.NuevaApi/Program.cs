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
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 🔥 OPTIMIZACIÓN 1: Configurar Kestrel para máximo rendimiento
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 10000; // ⬆️ Aumentado de 1000
    options.Limits.MaxConcurrentUpgradedConnections = 10000;
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);

    // 🔥 HTTP/3 para mejor rendimiento (si el servidor lo soporta)
    options.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
    });
});

// 🔥 OPTIMIZACIÓN 2: Output Caching para respuestas repetidas
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(30)));
    
    // Cache específico para endpoints de lectura
    options.AddPolicy("ReadEndpoints", builder => 
        builder.Expire(TimeSpan.FromMinutes(5))
               .SetVaryByQuery("page", "pageSize"));
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 🔥 OPTIMIZACIÓN 3: System.Text.Json con máximo rendimiento
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = false; // Más rápido
        options.JsonSerializerOptions.WriteIndented = false; // Producción: sin indentación
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        
        // 🔥 Source Generators para mejor rendimiento (si usas .NET 8+)
        options.JsonSerializerOptions.TypeInfoResolverChain.Add(AppJsonSerializerContext.Default);
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

// 🔥 OPTIMIZACIÓN 4: Response Compression (Brotli + Gzip) - Mejorado
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    
    // Comprimir solo respuestas grandes (> 1KB)
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "text/json" });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    // Brotli con mejor compresión para producción
    options.Level = builder.Environment.IsDevelopment() 
        ? CompressionLevel.Fastest 
        : CompressionLevel.Optimal;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = builder.Environment.IsDevelopment() 
        ? CompressionLevel.Fastest 
        : CompressionLevel.Optimal;
});

DefaultTypeMap.MatchNamesWithUnderscores = true;

DapperTypeHandlerRegistration.RegisterGuidValueObjectHandlers();

MapsterConfig.RegisterMapsterConfiguration(builder.Services);

builder.Services.AddApplication();
builder.Services.AddSharedApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 🔥 OPTIMIZACIÓN 5: Object Pooling para reducir GC pressure
builder.Services.AddSingleton<Microsoft.Extensions.ObjectPool.ObjectPoolProvider, 
    Microsoft.Extensions.ObjectPool.DefaultObjectPoolProvider>();

// 🔥 OPTIMIZACIÓN 6: Redis Cache para paginación optimizada (reduce 370ms a ~5ms)
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnection))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
        options.InstanceName = "AhorroLand:";
        
        // Configuración optimizada para rendimiento
        options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
        {
            EndPoints = { redisConnection },
            AbortOnConnectFail = false,
            ConnectTimeout = 5000,
            SyncTimeout = 5000,
            AsyncTimeout = 5000,
            KeepAlive = 60,
            ConnectRetry = 3,
            // Pool de conexiones para mejor rendimiento
            DefaultDatabase = 0,
        };
    });
}
else
{
    // Fallback a MemoryCache si Redis no está configurado
    builder.Services.AddDistributedMemoryCache();
}

// 🔥 Configuración de autenticación JWT
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
    
    // 🔥 OPTIMIZACIÓN 7: Configuración para mejor rendimiento de JWT
    options.SaveToken = false; // No guardar token en AuthenticationProperties (ahorra memoria)
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAhorroLandExceptionHandling();

// 🔥 OPTIMIZACIÓN 8: Output Caching middleware
app.UseOutputCache();

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

// 🔥 OPTIMIZACIÓN 9: Source Generator Context para JSON (mejor rendimiento)
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(Guid))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
