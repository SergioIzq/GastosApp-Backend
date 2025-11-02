using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace AhorroLand.Shared.Application;

public static class DependencyInyection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(typeof(DependencyInyection).Assembly);
        });

        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(typeof(DependencyInyection).Assembly);

        return services;
    }
}
