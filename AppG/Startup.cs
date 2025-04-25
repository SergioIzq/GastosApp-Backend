using NHibernate;
using NHibernate.Cfg;
using System.Reflection;
using AppG.Exceptions;
using AppG.Servicio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AppG.Middleware;
using Microsoft.AspNetCore.Authorization;
using AppG.Entidades.BBDD;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

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

                var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);

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
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
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
        app.UseCors("AllowAll");

        // Configuración de enrutamiento
        app.UseRouting();

        app.UseAuthentication(); // Añadir autenticación
        app.UseAuthorization();  // Añadir autorización

        // Configuración de puntos finales
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private ISessionFactory ConfigureNHibernate(IConfiguration configuration)
    {
        var cfg = new Configuration();        
        cfg.Configure("NHibernate/hibernate.cfg.xml"); // Ruta correcta al archivo XML
        cfg.AddAssembly(Assembly.GetExecutingAssembly());

        // Obtener la cadena de conexión desde appsettings.json o variable de entorno
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (connectionString.Contains("${DB_CONNECTION_STRING}"))
        {
            string? dbConnectionString = System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(dbConnectionString))
            {
                throw new InvalidOperationException("La variable de entorno DB_CONNECTION_STRING no está configurada.");
            }

            connectionString = connectionString.Replace("${DB_CONNECTION_STRING}", dbConnectionString);
        }

        // Establecer la cadena de conexión programáticamente
        cfg.SetProperty("connection.connection_string", connectionString);

        return cfg.BuildSessionFactory();
    }

}
