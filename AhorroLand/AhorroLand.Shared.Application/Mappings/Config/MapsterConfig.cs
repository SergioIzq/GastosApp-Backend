using AhorroLand.Domain;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AhorroLand.Infrastructure.Configuration;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

        // 2. Aplicar Configuraciones (Mapas Específicos)
        // Escanea las asambleas donde defines tus mapeos específicos (si usas IRegister)
        TypeAdapterConfig.GlobalSettings.Scan(
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(Cliente))!
        );

        // 3. Registrar la configuración como Singleton
        services.AddSingleton(TypeAdapterConfig.GlobalSettings);

        // 4. Registrar IAdapter (para usar .Adapt<T> fuera del DbContext)
        services.AddSingleton<IMapper, ServiceMapper>();
    }
}