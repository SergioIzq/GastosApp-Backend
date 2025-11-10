using AhorroLand.Shared.Domain.Abstractions.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace AhorroLand.Middleware;

/// <summary>
/// Middleware optimizado para manejo global de excepciones y objetos Result con errores.
/// Enfocado en rendimiento y mensajes claros para el usuario.
/// </summary>
public sealed class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    // ✅ OPTIMIZACIÓN: JsonSerializerOptions reutilizable (evita crear nuevas instancias en cada request)
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false // Más rápido sin indentación
    };

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // ✅ OPTIMIZACIÓN: Usar Stopwatch para medir el tiempo de respuesta de error
        var stopwatch = Stopwatch.StartNew();

        var (statusCode, errorResponse) = MapExceptionToResponse(exception);

        // Logging estructurado
        LogException(exception, context, statusCode);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        // ✅ OPTIMIZACIÓN: Serialización directa al stream (sin buffer intermedio)
        await JsonSerializer.SerializeAsync(context.Response.Body, errorResponse, JsonOptions);

        stopwatch.Stop();
        _logger.LogDebug("Respuesta de error enviada en {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
    }

    /// <summary>
    /// Mapea excepciones a respuestas HTTP con códigos de estado apropiados
    /// </summary>
    private (int statusCode, ErrorResponse response) MapExceptionToResponse(Exception exception)
    {
        return exception switch
        {
            // Excepciones de validación
            ArgumentNullException argNull =>
          (StatusCodes.Status400BadRequest,
                new ErrorResponse("Validation.ArgumentNull",
            "Argumento requerido",
                    $"El parámetro '{argNull.ParamName}' no puede ser nulo.")),

            ArgumentException arg =>
       (StatusCodes.Status400BadRequest,
      new ErrorResponse("Validation.ArgumentInvalid",
    "Argumento inválido",
       arg.Message)),

            // Operaciones inválidas
            InvalidOperationException invalid =>
                 (StatusCodes.Status400BadRequest,
             new ErrorResponse("Operation.Invalid",
            "Operación inválida",
                invalid.Message)),

            // Recursos no encontrados
            KeyNotFoundException notFound =>
              (StatusCodes.Status404NotFound,
           new ErrorResponse("Resource.NotFound",
                     "Recurso no encontrado",
                      notFound.Message)),

            // Acceso no autorizado
            UnauthorizedAccessException _ =>
     (StatusCodes.Status403Forbidden,
       new ErrorResponse("Access.Forbidden",
       "Acceso denegado",
      "No tienes permisos para realizar esta operación.")),

            // Timeout
            TimeoutException _ =>
        (StatusCodes.Status408RequestTimeout,
       new ErrorResponse("Request.Timeout",
       "Tiempo de espera agotado",
      "La operación tardó demasiado tiempo. Por favor, intenta nuevamente.")),

            // Operaciones no soportadas
            NotSupportedException notSupported =>
                  (StatusCodes.Status501NotImplemented,
         new ErrorResponse("Operation.NotSupported",
    "Operación no soportada",
    notSupported.Message)),

            // Error genérico (500)
            _ => (StatusCodes.Status500InternalServerError,
                   new ErrorResponse("Server.InternalError",
                     "Error interno del servidor",
            "Ocurrió un error inesperado. Por favor, contacta con soporte."))
        };
    }

    /// <summary>
    /// Logging estructurado optimizado
    /// </summary>
    private void LogException(Exception exception, HttpContext context, int statusCode)
    {
        var logLevel = statusCode >= 500 ? LogLevel.Error : LogLevel.Warning;

        _logger.Log(logLevel, exception,
  "Error manejado: {ExceptionType} - Status: {StatusCode} - Path: {Path} - Method: {Method} - TraceId: {TraceId}",
  exception.GetType().Name,
         statusCode,
       context.Request.Path,
         context.Request.Method,
   Activity.Current?.Id ?? context.TraceIdentifier);
    }
}

/// <summary>
/// Respuesta de error estandarizada
/// </summary>
public sealed record ErrorResponse
{
    public string Code { get; init; }
    public string Title { get; init; }
    public string Detail { get; init; }
    public string? TraceId { get; init; }
    public DateTime Timestamp { get; init; }

    public ErrorResponse(string code, string title, string detail, string? traceId = null)
    {
        Code = code;
        Title = title;
        Detail = detail;
        TraceId = traceId;
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Crea una respuesta de error desde un objeto Error del dominio
    /// </summary>
    public static ErrorResponse FromDomainError(Error error, string? traceId = null)
    {
        return new ErrorResponse(
         error.Code,
        error.Name,
             error.Message ?? "No se proporcionó detalle adicional.",
                  traceId
              );
    }
}
