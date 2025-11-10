using Microsoft.AspNetCore.Builder;

namespace AhorroLand.Middleware;

/// <summary>
/// Extensiones para registrar los middlewares de manejo de excepciones y resultados
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Registra el middleware de manejo global de excepciones
    /// DEBE ser uno de los primeros middlewares en el pipeline
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandler>();
    }

    /// <summary>
    /// Registra el middleware de manejo de objetos Result con errores
    /// DEBE registrarse DESPUÉS de UseRouting() pero ANTES de UseEndpoints()
    /// </summary>
    public static IApplicationBuilder UseResultHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ResultHandlerMiddleware>();
    }

    /// <summary>
    /// Registra ambos middlewares en el orden correcto
    /// </summary>
    public static IApplicationBuilder UseAhorroLandExceptionHandling(this IApplicationBuilder app)
    {
        app.UseGlobalExceptionHandler();
        app.UseResultHandler();
        return app;
    }
}
