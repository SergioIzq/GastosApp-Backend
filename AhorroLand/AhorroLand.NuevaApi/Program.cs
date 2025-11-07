using AhorroLand.Application;
using AhorroLand.Infrastructure;
using AhorroLand.Infrastructure.Configuration;
using AhorroLand.Infrastructure.TypesHandlers;
using AhorroLand.Shared.Application;
using Dapper;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.IO.Compression;

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
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// ?? USAR Response Compression
app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
