using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Authentication.Request;
using GuayaquilBank.Application.Dtos.Authentication.Response;
using GuayaquilBank.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuayaquilBank.WebApi.Controllers
{
    /// <summary>
    /// Controlador encargado de los procesos de autenticación, emisión de credenciales y seguridad del sistema.
    /// Proporciona los tokens de acceso JWT requeridos para consumir el resto de endpoints protegidos.
    /// </summary>
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthenticationAppService _appService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="AuthController"/>.
        /// </summary>
        /// <param name="appService">Servicio de aplicación encargado de la lógica de autenticación y validación de credenciales.</param>
        public AuthController(IAuthenticationAppService appService)
        {
            _appService = appService;
        }

        /// <summary>
        /// Autentica a un usuario en el sistema utilizando su nombre de usuario (Username) y contraseña.
        /// </summary>
        /// <remarks>
        /// Al autenticarse correctamente, el sistema retornará un token JWT (Bearer Token) junto con la información básica del perfil y el Tenant (CompanyId) al que pertenece.
        /// 
        /// Ejemplo de petición:
        /// 
        ///     POST /api/Auth/login
        ///     {
        ///        "domain": "guayaquilbank",
        ///        "username": "mbarcelona",
        ///        "password": "SecurePassword123!"
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">DTO que contiene las credenciales de acceso del usuario.</param>
        /// <returns>Un objeto de respuesta que incluye el Bearer Token generado y detalles de la sesión.</returns>
        /// <response code="200">Autenticación exitosa. Retorna el token JWT y datos de contexto del usuario.</response>
        /// <response code="400">Si los campos enviados no cumplen con los formatos requeridos.</response>
        /// <response code="401">Si las credenciales (usuario o contraseña) son incorrectas o el usuario está inactivo.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _appService.LoginAsync(request);

            // MODIFICADO: Envolvemos los datos usando tu método unificado
            return ToResponse(response, "Sesión iniciada correctamente de forma segura.");
        }
    }
}