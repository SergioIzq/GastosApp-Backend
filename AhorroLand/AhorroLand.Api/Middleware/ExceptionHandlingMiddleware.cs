namespace AppG.Middleware
{
    using AppG.Exceptions;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                var responseModel = new
                {
                    Succeeded = false,
                    Message = "Error desconocido.",
                    Errors = new List<string>()
                };

                switch (error)
                {
                    case ApiException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case ValidationException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel = new
                        {
                            Succeeded = false,
                            Message = "Error de validación.",
                            Errors = e.Errors.ToList()
                        };
                        break;
                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel = new
                        {
                            Succeeded = false,
                            Message = "Clave no encontrada.",
                            Errors = new List<string> { e.Message }
                        };
                        break;
                    case DirectoryNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel = new
                        {
                            Succeeded = false,
                            Message = "Directorio no encontrado.",
                            Errors = new List<string> { e.Message }
                        };
                        break;
                    case CustomUnauthorizedAccessException e:
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        responseModel = new
                        {
                            Succeeded = false,
                            Message = "Operación no autorizada.",
                            Errors = e.Errors.ToList()
                        };
                        break;
                    case FileNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        responseModel = new
                        {
                            Succeeded = false,
                            Message = "Operación no autorizada.",
                            Errors = new List<string> { "Fichero no encontrado." }
                        };
                        break;
                    case FileLoadException e:
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        responseModel = new
                        {
                            Succeeded = false,
                            Message = "Archivo inaccesible.",
                            Errors = new List<string> { "No se puede abrir el archivo, otro programa lo tiene abierto." }
                        };
                        break;
                    case IOException e:
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        responseModel = new
                        {
                            Succeeded = false,
                            Message = "Operación no autorizada.",
                            Errors = new List<string> { "Problema al intentar abrir el Excel generado, si lo tiene abierto, ciérrelo." }
                        };
                        break;
                    case SmtpException e:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel = new
                        {
                            Succeeded = false,
                            e.Message,
                            Errors = new List<string> { "No se ha podido enviar el correo contacte con el administrador para solucionarlo." }
                        };
                        break;
                    case UnauthorizedAccessException e:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        responseModel = new
                        {
                            Succeeded = false,
                            e.Message,
                            Errors = new List<string> { "Debe confirmar su correo antes de iniciar sesión." }
                        };
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel = new
                        {
                            Succeeded = false,
                            Message = "Error interno del servidor.",
                            Errors = new List<string> { "Ha ocurrido un error desconocido." }
                        };
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
