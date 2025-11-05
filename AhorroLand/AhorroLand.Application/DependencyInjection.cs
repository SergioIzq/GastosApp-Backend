using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace AhorroLand.Application
{
    public static class DependencyInyection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // 1. REGISTRA MEDIATR AQUÍ
            // Esto escanea 'AhorroLand.Application' y encuentra tus Handlers
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(DependencyInyection).Assembly)
            );

            // 2. REGISTRA MAPSTER AQUÍ TAMBIÉN
            // Esto escanea 'AhorroLand.Application' en busca de tus Mappings
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(DependencyInyection).Assembly);

            return services;
        }
    }
}