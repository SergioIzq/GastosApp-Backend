using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace AhorroLand.Shared.Application;

public static class DependencyInyection
{
    public static IServiceCollection AddSharedApplication(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(DependencyInyection).Assembly);

        return services;
    }
}
