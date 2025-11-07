using AhorroLand.Infrastructure.Configuration.Settings;
using AhorroLand.Infrastructure.DataAccess;
using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Infrastructure.Persistence.Warmup;
using AhorroLand.Infrastructure.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;

namespace AhorroLand.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 43));
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 1️⃣ DbContext
            services.AddDbContext<AhorroLandDbContext>(options =>
                options.UseMySql(connectionString, serverVersion));

            // 2️⃣ Cache distribuida (MemoryCache para desarrollo)
            services.AddDistributedMemoryCache();

            // 3️⃣ Dapper
            services.AddScoped<IDbConnection>(sp =>
                new MySqlConnection(configuration.GetConnectionString("DefaultConnection")));

            // 4️⃣ Email settings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // 5️⃣ Registro explícito de dependencias críticas
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<QueuedEmailService>();

            // 👉 Agrega esta línea clave:
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.AssignableTo(typeof(IWriteRepository<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );

            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.AssignableTo(typeof(IReadRepository<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );


            services.AddScoped<IDomainValidator, DapperDomainValidator>();


            // 6️⃣ Registro automático con Scrutor
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())

                // A) Servicios normales
                .AddClasses(classes => classes
                    .InNamespaces("AhorroLand.Infrastructure.Servicies")
                    .Where(c => !typeof(BackgroundService).IsAssignableFrom(c) && c.GetInterfaces().Length > 0)
                )
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                // B) BackgroundServices
                .AddClasses(classes => classes.AssignableTo<BackgroundService>())
                    .AsSelf()
                    .As<IHostedService>()
                    .WithSingletonLifetime()
            );

            services.AddScoped<IDbConnectionFactory, SqlDbConnectionFactory>();

            // 🔥 Warm-up de conexiones al iniciar
            services.AddHostedService<DatabaseWarmupService>();

            return services;
        }
    }
}
