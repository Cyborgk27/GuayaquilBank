using System.Threading.Tasks;
using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Profile.Request;
using GuayaquilBank.Application.Dtos.Profile.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuayaquilBank.WebApi.Controllers
{
    /// <summary>
    /// Controlador centralizado para que el usuario en sesión gestione sus datos personales y la configuración global de su empresa.
    /// Resuelve de forma segura las identidades mediante el procesamiento del Bearer Token JWT.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
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
        [ProducesResponseType(typeof(MyUserResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProfileAsync()
        {
            var result = await _profileService.GetMyProfileAsync();
            return Ok(result);
        }

        /// <summary>
        /// Actualiza la información de perfil (Email y Foto de Perfil) del operador actual.
        /// </summary>
        /// <response code="200">El perfil ha sido modificado y guardado con éxito.</response>
        /// <response code="400">Si el correo ingresado ya pertenece a otra cuenta en la plataforma.</response>
        [HttpPut]
        [ProducesResponseType(typeof(MyUserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMyProfileAsync([FromBody] UpdateMyProfileRequestDto request)
        {
            var result = await _profileService.UpdateMyProfileAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Recupera la configuración comercial, localización y parámetros financieros (IVA, Moneda) de la empresa del usuario actual.
        /// </summary>
        /// <response code="200">Retorna los parámetros de configuración del Tenant actual.</response>
        /// <response code="404">Si el usuario no está asociado a una empresa válida.<///response>
        [HttpGet("company")]
        [ProducesResponseType(typeof(MyCompanyResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyCompanyAsync()
        {
            var result = await _profileService.GetMyCompanyAsync();
            return Ok(result);
        }

        /// <summary>
        /// Actualiza la configuración comercial de la empresa actual, incluyendo direcciones físicas, contacto y tasas fiscales (IVA).
        /// </summary>
        /// <remarks>
        /// Este endpoint impactará la información impresa de las facturas electrónicas y PDFs emitidos a partir del guardado.
        /// </remarks>
        /// <response code="200">Configuración corporativa guardada con éxito.</response>
        /// <response code="400">Si los datos comerciales fallan validaciones de unicidad fiscal.<///response>
        [HttpPut("company")]
        [ProducesResponseType(typeof(MyCompanyResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMyCompanyAsync([FromBody] UpdateMyCompanyRequestDto request)
        {
            var result = await _profileService.UpdateMyCompanyAsync(request);
            return Ok(result);
        }
    }
}