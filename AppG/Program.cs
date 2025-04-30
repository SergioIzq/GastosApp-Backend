using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                      .AddEnvironmentVariables();
            })

            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.ListenAnyIP(80); // HTTP opcional
                    serverOptions.ListenAnyIP(443, listenOptions =>
                    {
                        listenOptions.UseHttps("/etc/letsencrypt/live/gbackend.sergioizq.es/certificate.pfx", "Bayron2004");
                    });
                });
                var httpClientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate, chain, sslPolicyErrors) => true
                };

                var httpClient = new HttpClient(httpClientHandler);
                webBuilder.UseStartup<Startup>(); // Usar Startup.cs para la configuración
            });
}
