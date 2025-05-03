using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AppG.Entidades.BBDD;
using CronJobProgramado;
using Hangfire;
using Hangfire.PostgreSql;
using NHibernate;
using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using NHibernate.Cfg;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
        // Inicializar NHibernate
        var sessionFactory = ConfigureNHibernate(_configuration);
        services.AddSingleton(sessionFactory);

        // Registrar lógica del job
        services.AddScoped<GastoProgramadoJob>();

        // Configurar Hangfire
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (connectionString != null && connectionString.Contains("${DB_CONNECTION_STRING}"))
        {
            string? dbConnectionString = System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(dbConnectionString))
            {
                throw new InvalidOperationException("La variable de entorno DB_CONNECTION_STRING no está configurada.");
            }

            connectionString = connectionString.Replace("${DB_CONNECTION_STRING}", dbConnectionString);
        }

        services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage(
                options => options.UseNpgsqlConnection(connectionString),
                new PostgreSqlStorageOptions
                {
                    SchemaName = "hangfire",
                    QueuePollInterval = TimeSpan.FromSeconds(15)
                });
        });

        services.AddHangfireServer();        
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseHangfireDashboard();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapPost("/api/cronjob/programar", async context =>
            {
                try
                {
                    var gasto = await JsonSerializer.DeserializeAsync<GastoProgramado>(
                        context.Request.Body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (gasto == null)
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("JSON inválido.");
                        return;
                    }

                    var jobService = context.RequestServices.GetRequiredService<GastoProgramadoJob>();
                    var creado = await jobService.ProgramarGastoAsync(gasto);

                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync($"Gasto programado con ID: {creado.Id}");
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync($"Error: {ex.Message}");
                }
            });
        });
    }

    private ISessionFactory ConfigureNHibernate(IConfiguration configuration)
    {
        Console.WriteLine("Environment: " + System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

        var cfg = new Configuration();
        cfg.Configure("NHibernate/hibernate.cfg.xml"); // Ruta correcta al archivo XML
        cfg.AddAssembly(Assembly.GetExecutingAssembly());

        // Obtener la cadena de conexión desde appsettings.json o variable de entorno
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (connectionString != null && connectionString.Contains("${DB_CONNECTION_STRING}"))
        {
            string? dbConnectionString = System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(dbConnectionString))
            {
                throw new InvalidOperationException("La variable de entorno DB_CONNECTION_STRING no está configurada.");
            }

            connectionString = connectionString.Replace("${DB_CONNECTION_STRING}", dbConnectionString);
        }

        // Establecer la cadena de conexión programáticamente
        cfg.SetProperty("connection.connection_string", connectionString);

        return cfg.BuildSessionFactory();
    }
}
