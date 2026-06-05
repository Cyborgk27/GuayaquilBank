using GuayaquilBank.Domain.Exceptions;
using GuayaquilBank.WebApi.Models;
using System.Net;
using System.Text.Json;

namespace GuayaquilBank.WebApi.Middlewares
{
    /// <summary>
    /// Middleware encargado de interceptar todas las excepciones no manejadas del pipeline HTTP,
    /// formateando las salidas bajo el estándar unificado de <see cref="ApiResponse{T}"/>.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
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

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>
            {
                Success = false,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case UserFriendlyException userEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Code = (int)HttpStatusCode.BadRequest;
                    response.Message = userEx.Message;
                    _logger.LogWarning("Advertencia de negocio controlada: {Message}", userEx.Message);
                    break;

                case ArgumentException argEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Code = (int)HttpStatusCode.BadRequest;
                    response.Message = "Uno o más parámetros de la solicitud son inválidos.";
                    response.Errors = new List<string> { argEx.Message };
                    _logger.LogWarning(argEx, "Falla en los argumentos de la petición.");
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Code = (int)HttpStatusCode.InternalServerError;
                    response.Message = "Ocurrió un error inesperado en nuestro servidor. Por favor, intente más tarde.";

                    if (_env.IsDevelopment())
                    {
                        response.Errors = new List<string> { exception.Message, exception.StackTrace ?? string.Empty };
                    }

                    _logger.LogError(exception, "Error crítico no manejado detectado en el pipeline: {Message}", exception.Message);
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonResult = JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsync(jsonResult);
        }
    }
}