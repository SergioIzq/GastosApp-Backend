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
                    // No usamos HTTPS en Kestrel, ya que Nginx lo manejará
                    serverOptions.ListenAnyIP(5000); // Solo HTTP en el puerto 5000
                });

                // Configura la aplicación para usar Startup.cs
                webBuilder.UseStartup<Startup>();
            });
}
