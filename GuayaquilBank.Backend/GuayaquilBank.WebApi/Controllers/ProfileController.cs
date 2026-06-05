using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Profile.Request;
using GuayaquilBank.Application.Dtos.Profile.Response;
using GuayaquilBank.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuayaquilBank.WebApi.Controllers
{
    /// <summary>
    /// Controlador centralizado para que el usuario en sesión gestione sus datos personales y la configuración global de su empresa.
    /// Resuelve de forma segura las identidades mediante el procesamiento del Bearer Token JWT.
    /// </summary>
    [Route("api/[controller]")]
    public class ProfileController : BaseApiController
    {
        private readonly IProfileAppService _profileService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ProfileController"/>.
        /// </summary>
        public ProfileController(IProfileAppService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Obtiene el perfil de cuenta detallado del usuario autenticado en la sesión.
        /// </summary>
        /// <response code="200">Retorna los datos informativos del operador en sesión.</response>
        /// <response code="401">Si el token no es enviado o ha expirado.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<MyUserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyProfileAsync()
        {
            var result = await _profileService.GetMyProfileAsync();

            return ToResponse(result, "Perfil de usuario cargado correctamente.");
        }

        /// <summary>
        /// Actualiza la información de perfil (Email y Foto de Perfil) del operador actual.
        /// </summary>
        /// <response code="200">El perfil ha sido modificado y guardado con éxito.</response>
        /// <response code="400">Si el correo ingresado ya pertenece a otra cuenta en la plataforma.</response>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<MyUserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMyProfileAsync([FromBody] UpdateMyProfileRequestDto request)
        {
            var result = await _profileService.UpdateMyProfileAsync(request);

            return ToResponse(result, "Tu información de perfil ha sido actualizada con éxito.");
        }

        /// <summary>
        /// Recupera la configuración comercial, localización y parámetros financieros (IVA, Moneda) de la empresa del usuario actual.
        /// </summary>
        /// <response code="200">Retorna los parámetros de configuración del Tenant actual.</response>
        /// <response code="404">Si el usuario no está asociado a una empresa válida.</response>
        [HttpGet("company")]
        [ProducesResponseType(typeof(ApiResponse<MyCompanyResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyCompanyAsync()
        {
            var result = await _profileService.GetMyCompanyAsync();

            return ToResponse(result, "Datos comerciales de la empresa recuperados con éxito.");
        }

        /// <summary>
        /// Actualiza la configuración comercial de la empresa actual, incluyendo direcciones físicas, contacto y tasas fiscales (IVA).
        /// </summary>
        /// <remarks>
        /// Este endpoint impactará la información impresa de las facturas electrónicas y PDFs emitidos a partir del guardado.
        /// </remarks>
        /// <response code="200">Configuración corporativa guardada con éxito.</response>
        /// <response code="400">Si los datos comerciales fallan validaciones de unicidad fiscal.</response>
        [HttpPut("company")]
        [ProducesResponseType(typeof(ApiResponse<MyCompanyResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMyCompanyAsync([FromBody] UpdateMyCompanyRequestDto request)
        {
            var result = await _profileService.UpdateMyCompanyAsync(request);

            return ToResponse(result, "Los parámetros de la configuración empresarial se guardaron correctamente.");
        }
    }
}