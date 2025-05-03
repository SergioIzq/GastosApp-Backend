using NHibernate;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AppG.Middleware;
using Microsoft.AspNetCore.Authorization;
using Hangfire;
using Hangfire.PostgreSql;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        ConnectionString = GetConnectionString(Configuration);
    }

    public IConfiguration Configuration { get; }
    public string ConnectionString = string.Empty;

    public void ConfigureServices(IServiceCollection services)
    {
        var assembly = typeof(Startup).Assembly;

        var tipos = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Servicio"));

        foreach (var impl in tipos)
        {
            var interfaz = impl.GetInterfaces().FirstOrDefault(i => i.Name == $"I{impl.Name}");
            if (interfaz != null)
            {
                services.AddScoped(interfaz, impl);
            }
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {

                var keyString = Configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(keyString))
                {
                    throw new InvalidOperationException("La clave JWT no está configurada en la configuración.");
                }
                var key = Encoding.UTF8.GetBytes(keyString);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        services.AddAuthorization(auth =>
        {
            auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                .RequireAuthenticatedUser().Build());
        });
        // Configuración de CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAppG", builder =>
                builder.WithOrigins("https://ahorroland.sergioizq.es", "http://localhost:4200")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials());
        });

        // Configuración de NHibernate
        var sessionFactory = ConfigureNHibernate(Configuration);
        services.AddSingleton<ISessionFactory>(sessionFactory);

        // Configuración de servicios de API
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers()
        .AddJsonOptions(options =>
        {
            // Configura la política de nombres de propiedad (cambiar a 'camelCase' si es necesario)
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

        services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage(
                options => options.UseNpgsqlConnection(ConnectionString),
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
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Configuración de Swagger
        app.UseSwagger();
        app.UseSwaggerUI();

        // Configuración de CORS
        app.UseCors("AllowAppG");

        // Configuración de enrutamiento
        app.UseRouting();

        app.UseAuthentication(); // Añadir autenticación
        app.UseAuthorization();  // Añadir autorización

        app.UseHangfireDashboard("/hangfire");

        // Configuración de puntos finales
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private ISessionFactory ConfigureNHibernate(IConfiguration configuration)
    {
        var cfg = new NHibernate.Cfg.Configuration();
        cfg.Configure("NHibernate/hibernate.cfg.xml");
        cfg.AddAssembly(Assembly.GetExecutingAssembly());

        // Establecer la cadena de conexión programáticamente
        cfg.SetProperty("connection.connection_string", ConnectionString);

        return cfg.BuildSessionFactory();
    }

    private string GetConnectionString(IConfiguration configuration)
    {
        // Obtener la cadena de conexión desde appsettings.json o variable de entorno
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
        }

        if (connectionString.Contains("${DB_CONNECTION_STRING}"))
        {
            var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(dbConnectionString))
            {
                throw new InvalidOperationException("La variable de entorno DB_CONNECTION_STRING no está configurada.");
            }

            connectionString = connectionString.Replace("${DB_CONNECTION_STRING}", dbConnectionString);
        }

        return connectionString;
    }

}
