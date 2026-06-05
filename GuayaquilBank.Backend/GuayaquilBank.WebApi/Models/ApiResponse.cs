namespace GuayaquilBank.WebApi.Models
{
    /// <summary>
    /// Estructura estándar y unificada para todas las respuestas HTTP de la API de Guayaquil Bank.
    /// </summary>
    /// <typeparam name="T">Tipo de dato que contiene la propiedad Data.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica si la operación se ejecutó con éxito.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Código de estado HTTP o código interno de negocio.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Mensaje informativo sobre el resultado de la operación.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Datos devueltos por la solicitud (en caso de éxito).
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Listado de errores detallados (común en fallas de validación).
        /// </summary>
        public IEnumerable<string>? Errors { get; set; }

        /// <summary>
        /// Fecha y hora exacta en la que se procesó la respuesta (ISO 8601).
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operación realizada con éxito.", int code = 200)
        {
            return new ApiResponse<T> { Success = true, Code = code, Message = message, Data = data };
        }

        public static ApiResponse<T> FailureResponse(string message, IEnumerable<string>? errors = null, int code = 400)
        {
            return new ApiResponse<T> { Success = false, Code = code, Message = message, Errors = errors };
        }
    }
}