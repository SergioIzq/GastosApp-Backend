using AppG.Middleware;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using NHibernate;
using System.Reflection;
using System.Text;
using System.Text.Json;

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

        services.AddHttpContextAccessor();
        services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
        services.AddScoped<EmailService>();

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

                ConfigureJwtBearerEvents(options);
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
            options.AddPolicy("SergioIzqDomain", builder =>
            {
                builder.SetIsOriginAllowed(origin =>
                {
                    // Intenta parsear el origen
                    if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    {
                        return false; // No es un origen válido
                    }

                    // Permite localhost:4200 para desarrollo
                    if (uri.Host == "localhost" && uri.Port == 4200)
                    {
                        return true;
                    }

                    // Permite el dominio raíz (sergioizq.com) O cualquier subdominio (.sergioizq.com)
                    return uri.Host == "sergioizq.com" || uri.Host.EndsWith(".sergioizq.com");
                })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
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
            config.UseStorage(
                new MySqlStorage(
                    ConnectionString, // tu cadena de conexión MySQL
                    new MySqlStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        // No hay esquema en MySQL, pero puedes usar prefijos en tablas si quieres
                        TablesPrefix = "hangfire_"
                    }));
        });

        services.AddHangfireServer();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configuración de CORS
        app.UseCors("SergioIzqDomain");

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
        app.UseCors("SergioIzqDomain");
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

    private static void ConfigureJwtBearerEvents(JwtBearerOptions options)
    {
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse(); // Evita la respuesta automática

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Succeeded = false,
                    Message = "Sesión expirada o token inválido.",
                    Errors = new[] { "Tu sesión ha expirado. Por favor, inicia sesión nuevamente." }
                };

                var json = JsonSerializer.Serialize(response);
                return context.Response.WriteAsync(json);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Succeeded = false,
                    Message = "Acceso denegado.",
                    Errors = new[] { "No tienes permisos para acceder a este recurso." }
                };

                var json = JsonSerializer.Serialize(response);
                return context.Response.WriteAsync(json);
            }
        };
    }

}
