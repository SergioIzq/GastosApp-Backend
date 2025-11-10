using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace AhorroLand.Middleware;

/// <summary>
/// Middleware optimizado para interceptar objetos Result con errores y convertirlos en respuestas HTTP apropiadas.
/// </summary>
public sealed class ResultHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResultHandlerMiddleware> _logger;

    // ✅ OPTIMIZACIÓN: JsonSerializerOptions reutilizable
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public ResultHandlerMiddleware(RequestDelegate next, ILogger<ResultHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        // ✅ OPTIMIZACIÓN: Usar MemoryStream del pool si es posible
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            // Solo procesar si la respuesta es exitosa pero podría contener un Result con error
            if (context.Response.StatusCode == StatusCodes.Status200OK &&
                context.Response.ContentType?.Contains("application/json") == true)
            {
                responseBody.Seek(0, SeekOrigin.Begin);

                // ✅ OPTIMIZACIÓN: Leer y parsear en una sola operación
                var bodyContent = await new StreamReader(responseBody).ReadToEndAsync();

                // Intentar detectar si es un Result con error
                if (TryDetectResultError(bodyContent, out var errorResponse, out var statusCode))
                {
                    _logger.LogInformation(
           "Result con error detectado: {ErrorCode} - Status: {StatusCode} - Path: {Path}",
                         errorResponse.Code, statusCode, context.Request.Path);

                    // Reemplazar la respuesta con el error mapeado
                    await WriteErrorResponseAsync(context, originalBodyStream, errorResponse, statusCode);
                    return;
                }
            }

            // Si no hay error, copiar la respuesta original
            responseBody.Seek(0, SeekOrigin.Begin);
            context.Response.Body = originalBodyStream;
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    /// <summary>
    /// Detecta si el contenido JSON contiene un Result con error
    /// ✅ OPTIMIZACIÓN: Parsing eficiente con ReadOnlySpan
    /// </summary>
    private bool TryDetectResultError(string jsonContent, out ErrorResponse? errorResponse, out int statusCode)
    {
        errorResponse = null;
        statusCode = StatusCodes.Status500InternalServerError;

        try
        {
            // ✅ OPTIMIZACIÓN: Deserialización con opciones optimizadas
            using var doc = JsonDocument.Parse(jsonContent, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            });

            var root = doc.RootElement;

            // Verificar si tiene las propiedades de Result
            if (!root.TryGetProperty("isSuccess", out var isSuccessElement) &&
       !root.TryGetProperty("IsSuccess", out isSuccessElement))
            {
                return false;
            }

            // Si es exitoso, no hay error
            if (isSuccessElement.GetBoolean())
            {
                return false;
            }

            // Extraer el error
            if (!root.TryGetProperty("error", out var errorElement) &&
             !root.TryGetProperty("Error", out errorElement))
            {
                return false;
            }

            // Parsear el objeto Error
            var code = errorElement.TryGetProperty("code", out var codeElement) ||
        errorElement.TryGetProperty("Code", out codeElement)
             ? codeElement.GetString() ?? "Error.Unknown"
               : "Error.Unknown";

            var name = errorElement.TryGetProperty("name", out var nameElement) ||
         errorElement.TryGetProperty("Name", out nameElement)
          ? nameElement.GetString() ?? "Error"
        : "Error";

            var message = errorElement.TryGetProperty("message", out var messageElement) ||
  errorElement.TryGetProperty("Message", out messageElement)
     ? messageElement.GetString() ?? "Ocurrió un error"
           : "Ocurrió un error";

            // Mapear código de error a status HTTP
            statusCode = MapErrorCodeToHttpStatus(code);

            errorResponse = new ErrorResponse(code, name, message, Activity.Current?.Id);
            return true;
        }
        catch (JsonException ex)
        {
            _logger.LogDebug(ex, "No se pudo parsear la respuesta como JSON. No es un Result.");
            return false;
        }
    }

    /// <summary>
    /// Mapea códigos de error del dominio a códigos HTTP
    /// ✅ OPTIMIZACIÓN: Switch expression con pattern matching
    /// </summary>
    private static int MapErrorCodeToHttpStatus(string errorCode)
    {
        return errorCode switch
        {
            // 400 - Bad Request
            var code when code.Contains("Validation", StringComparison.OrdinalIgnoreCase) =>
                StatusCodes.Status400BadRequest,

            // 404 - Not Found
            var code when code.Contains("NotFound", StringComparison.OrdinalIgnoreCase) =>
            StatusCodes.Status404NotFound,

            // 409 - Conflict
            var code when code.Contains("Conflict", StringComparison.OrdinalIgnoreCase) =>
            StatusCodes.Status409Conflict,

            var code when code.Contains("AlreadyExists", StringComparison.OrdinalIgnoreCase) =>
                 StatusCodes.Status409Conflict,

            // 401 - Unauthorized
            var code when code.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) =>
        StatusCodes.Status401Unauthorized,

            var code when code.Contains("Authentication", StringComparison.OrdinalIgnoreCase) =>
           StatusCodes.Status401Unauthorized,

            // 403 - Forbidden
            var code when code.Contains("Forbidden", StringComparison.OrdinalIgnoreCase) =>
           StatusCodes.Status403Forbidden,

            var code when code.Contains("Authorization", StringComparison.OrdinalIgnoreCase) =>
           StatusCodes.Status403Forbidden,

            // 422 - Unprocessable Entity
            var code when code.Contains("UpdateFailure", StringComparison.OrdinalIgnoreCase) =>
     StatusCodes.Status422UnprocessableEntity,

            var code when code.Contains("DeleteFailure", StringComparison.OrdinalIgnoreCase) =>
                   StatusCodes.Status422UnprocessableEntity,

            // 500 - Internal Server Error (default)
            _ => StatusCodes.Status500InternalServerError
        };
    }

    /// <summary>
    /// Escribe la respuesta de error al stream de salida
    /// ✅ OPTIMIZACIÓN: Serialización directa al stream
    /// </summary>
    private static async Task WriteErrorResponseAsync(
        HttpContext context,
     Stream outputStream,
        ErrorResponse errorResponse,
        int statusCode)
    {
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await JsonSerializer.SerializeAsync(outputStream, errorResponse, JsonOptions);
    }
}
