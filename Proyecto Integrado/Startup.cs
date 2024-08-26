using NHibernate;
using NHibernate.Cfg;
using System.Reflection;
using AppG.Exceptions;
using AppG.Servicio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AppG.Middleware;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddScoped<IGastoServicio, GastoServicio>();
        services.AddScoped<IIngresoServicio, IngresoServicio>();
        services.AddScoped<ICategoriaServicio, CategoriaServicio>();
        services.AddScoped<IConceptoServicio, ConceptoServicio>();
        services.AddScoped<IResumenServicio, ResumenServicio>();
        services.AddScoped<IPersonaServicio, PersonaServicio>();
        services.AddScoped<IClienteServicio, ClienteServicio>();
        services.AddScoped<IProveedorServicio, ProveedorServicio>();
        services.AddScoped<ICuentaServicio, CuentaServicio>();
        services.AddScoped<ITraspasoServicio, TraspasoServicio>();
        services.AddScoped<IFormaPagoServicio, FormaPagoServicio>();
        services.AddScoped<IUsuarioServicio, UsuarioServicio>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

        services.AddAuthorization();

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
        cfg.Configure("NHibernate/hibernate.cfg.xml");
        cfg.AddAssembly(Assembly.GetExecutingAssembly());
        return cfg.BuildSessionFactory();
    }
}
