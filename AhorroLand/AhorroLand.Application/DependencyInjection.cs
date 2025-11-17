using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace AhorroLand.Application
{
    public static class DependencyInyection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(DependencyInyection).Assembly)
            );

            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(DependencyInyection).Assembly);

            return services;
        }
    }
}