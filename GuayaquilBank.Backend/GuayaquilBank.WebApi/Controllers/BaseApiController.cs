using GuayaquilBank.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuayaquilBank.WebApi.Controllers
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Envuelve los datos de éxito en el formato estándar unificado ApiResponse.
        /// </summary>
        protected ObjectResult ToResponse<T>(T data, string message = "Operación exitosa", int statusCode = 200)
        {
            var response = ApiResponse<T>.SuccessResponse(data, message, statusCode);
            return StatusCode(statusCode, response);
        }
    }
}