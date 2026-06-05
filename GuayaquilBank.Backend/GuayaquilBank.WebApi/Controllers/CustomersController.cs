using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Sales.Request;
using GuayaquilBank.Application.Dtos.Sales.Response;
using GuayaquilBank.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuayaquilBank.WebApi.Controllers
{
    /// <summary>
    /// Controlador para la gestión y administración de clientes (Customers) dentro del módulo de ventas.
    /// Garantiza el aislamiento por Tenant de manera transparente basándose en la sesión del usuario actual.
    /// </summary>
    [Route("api/[controller]")]
    public class CustomersController : BaseApiController
    {
        private readonly ICustomerAppService _customerService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="CustomersController"/>.
        /// </summary>
        /// <param name="customerService">Servicio de aplicación encargado de la lógica de negocio de clientes.</param>
        public CustomersController(ICustomerAppService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Registra un nuevo cliente en el sistema asociado al Tenant actual.
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        /// 
        ///     POST /api/Customers
        ///     {
        ///        "identification": "0999999999001",
        ///        "fullName": "Juan Pérez",
        ///        "email": "juan.perez@example.com",
        ///        "phoneNumber": "0987654321",
        ///        "address": "Av. Principal y Calle 2"
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">Datos necesarios para la creación del cliente.</param>
        /// <returns>El cliente recién creado con su ID asignado.</returns>
        /// <response code="201">Retorna el cliente creado exitosamente junto con la cabecera Location.</response>
        /// <response code="400">Si los datos enviados fallan las validaciones o la identificación ya está registrada.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerRequestDto request)
        {
            var result = await _customerService.CreateAsync(request);

            var response = ApiResponse<CustomerResponseDto>.SuccessResponse(result, "Cliente registrado exitosamente.", StatusCodes.Status201Created);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, response);
        }

        /// <summary>
        /// Obtiene un listado paginado y filtrado de los clientes pertenecientes a la empresa del usuario.
        /// </summary>
        /// <param name="request">Parámetros de paginación y criterios de búsqueda (búsqueda por Nombre, Identificación o Email).</param>
        /// <returns>Colección paginada de clientes que coinciden con los criterios establecidos.</returns>
        /// <response code="200">Retorna la estructura de paginación con la lista de clientes solicitada.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponseDto<CustomerResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedAsync([FromQuery] PaginationRequestDto request)
        {
            var result = await _customerService.GetPagedAsync(request);
            return ToResponse(result, "Listado de clientes recuperado con éxito.");
        }

        /// <summary>
        /// Obtiene la información detallada de un cliente específico mediante su identificador único.
        /// </summary>
        /// <param name="id">Identificador único (GUID) del cliente.</param>
        /// <returns>El DTO detallado del cliente localizado.</returns>
        /// <response code="200">Retorna el cliente encontrado.</response>
        /// <response code="404">Si el cliente no existe o no pertenece al Tenant del usuario autenticado.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _customerService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse($"El cliente con ID {id} no fue localizado en este Tenant.", null, StatusCodes.Status404NotFound));
            }

            return ToResponse(result, "Cliente localizado.");
        }

        /// <summary>
        /// Actualiza los datos de perfil e identificación de un cliente existente.
        /// </summary>
        /// <param name="id">Identificador único (GUID) del cliente a modificar.</param>
        /// <param name="request">Nuevos datos para actualizar el perfil del cliente.</param>
        /// <returns>El cliente con los cambios aplicados y guardados.</returns>
        /// <response code="200">Retorna el cliente modificado de manera exitosa.</response>
        /// <response code="400">Si los datos de actualización son inválidos o la nueva identificación colisiona con otro cliente.</response>
        /// <response code="404">Si el cliente solicitado no existe en los registros de la empresa.</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateCustomerRequestDto request)
        {
            var result = await _customerService.UpdateAsync(id, request);
            return ToResponse(result, "Los datos del cliente se actualizaron de forma satisfactoria.");
        }

        /// <summary>
        /// Realiza un borrado lógico (Soft Delete) de un cliente en el sistema.
        /// </summary>
        /// <param name="id">Identificador único (GUID) del cliente a dar de baja de forma lógica.</param>
        /// <returns>Ningún contenido en caso de éxito.</returns>
        /// <response code="204">Indica que el cliente fue marcado como eliminado correctamente.</response>
        /// <response code="404">Si el cliente no existe en el Tenant actual.</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _customerService.DeleteAsync(id);
            return ToResponse((object?)null, "Cliente removido del sistema operativo con éxito.", StatusCodes.Status204NoContent);
        }

        /// <summary>
        /// Alterna de forma atómica el estado de actividad (Activo/Inactivo) de un cliente específico.
        /// </summary>
        /// <param name="id">Identificador único (GUID) del cliente.</param>
        /// <returns>Ningún contenido en caso de éxito.</returns>
        /// <response code="204">El estado del cliente cambió y se guardó exitosamente.</response>
        /// <response code="404">Si el cliente no se encuentra registrado dentro de la empresa.</response>
        [HttpPatch("{id:guid}/toggle-status")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleStatusAsync(Guid id)
        {
            await _customerService.ToggleStatusAsync(id);
            return ToResponse((object?)null, "El estado comercial del cliente fue conmutado con éxito.", StatusCodes.Status204NoContent);
        }
    }
}