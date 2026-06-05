using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Identity.Request;
using GuayaquilBank.Application.Dtos.Identity.Response;
using Microsoft.AspNetCore.Mvc;

namespace GuayaquilBank.WebApi.Controllers
{
    /// <summary>
    /// Controlador para la gestión, control de accesos e identidad de usuarios del sistema.
    /// Mantiene de forma estricta el aislamiento por Tenant (Empresa) según el contexto del usuario autenticado.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserAppService _userService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UsersController"/>.
        /// </summary>
        /// <param name="userService">Servicio de aplicación encargado de la lógica y utilerías de identidad.</param>
        public UsersController(IUserAppService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema operativo bajo el Tenant actual.
        /// </summary>
        /// <remarks>
        /// El password ingresado será encriptado automáticamente mediante un hash seguro en la capa de servicios.
        /// 
        /// Ejemplo de petición:
        /// 
        ///     POST /api/Users
        ///     {
        ///        "username": "mbarcelona",
        ///        "password": "SecurePassword123!",
        ///        "email": "mbarcelona@guayaquilbank.com",
        ///        "firstName": "Manuel",
        ///        "lastName": "Barcelona",
        ///        "profilePictureUrl": "https://storage.guayaquilbank.com/profiles/avatar.png"
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">DTO con los datos credenciales y de perfil para el nuevo usuario.</param>
        /// <returns>Los datos del usuario creado, excluyendo información sensible de credenciales.</returns>
        /// <response code="201">Retorna el usuario registrado exitosamente con su ID generado.</response>
        /// <response code="400">Si el Username o Email ya existen de manera global o los campos no cumplen las validaciones.</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserRequestDto request)
        {
            var result = await _userService.CreateAsync(request);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
        }

        /// <summary>
        /// Recupera un listado paginado, ordenado y filtrado de los usuarios que pertenecen al mismo Tenant.
        /// </summary>
        /// <param name="request">Criterios de paginación (Page, PageSize) y cadenas de texto para búsqueda (Username, Nombre, Apellido, Email).</param>
        /// <returns>Objeto de paginación con la lista filtrada de usuarios.</returns>
        /// <response code="200">Retorna la colección paginada solicitada.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedAsync([FromQuery] PaginationRequestDto request)
        {
            var result = await _userService.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el perfil detallado de un usuario específico a través de su identificador único (GUID).
        /// </summary>
        /// <param name="id">ID único del usuario en formato GUID.</param>
        /// <returns>Los datos de perfil mapeados en un DTO seguro.</returns>
        /// <response code="200">Retorna el usuario localizado con éxito.</response>
        /// <response code="404">Si el ID no existe en los registros o el usuario pertenece a otra organización corporativa.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { Message = $"El usuario con ID {id} no fue localizado en este Tenant." });
            }
            return Ok(result);
        }

        /// <summary>
        /// Actualiza la información de perfil, correo electrónico e imagen de un usuario operativo.
        /// </summary>
        /// <param name="id">ID del usuario a modificar.</param>
        /// <param name="request">Nuevos valores para aplicar al perfil del usuario.</param>
        /// <returns>El DTO modificado del usuario reflejando los cambios confirmados.</returns>
        /// <response code="200">Retorna el usuario actualizado de forma exitosa.</response>
        /// <response code="400">Si el correo electrónico ingresado ya está en uso por otro miembro o los formatos son inválidos.</response>
        /// <response code="404">Si el usuario a editar no pertenece al entorno de la empresa del solicitante.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateUserRequestDto request)
        {
            var result = await _userService.UpdateAsync(id, request);
            return Ok(result);
        }

        /// <summary>
        /// Ejecuta la remoción física/lógica (Soft Delete) de un usuario, manteniendo la integridad del registro de auditoría.
        /// </summary>
        /// <param name="id">ID del usuario a dar de baja de la base de datos activa.</param>
        /// <returns>Sin contenido (No Content) ante la baja exitosa.</returns>
        /// <response code="204">Confirma que el usuario ha sido marcado como eliminado lógicamente.</response>
        /// <response code="404">Si el usuario no fue localizado en el Tenant actual.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Conmuta de manera atómica el estado de actividad de un usuario interno (Activa o Desactiva según su estado actual).
        /// </summary>
        /// <remarks>
        /// Útil para bloquear accesos temporales al sistema ERP sin necesidad de borrar lógicamente al operador.
        /// </remarks>
        /// <param name="id">ID del usuario del sistema.</param>
        /// <returns>Sin contenido en caso de un cambio exitoso.</returns>
        /// <response code="204">El estado del usuario se actualizó de forma satisfactoria en la base de datos.</response>
        /// <response code="404">Si el operador no existe bajo los alcances de la empresa autenticada.</response>
        [HttpPatch("{id:guid}/toggle-status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleStatusAsync(Guid id)
        {
            await _userService.ToggleStatusAsync(id);
            return NoContent();
        }
    }
}