using System;
using System.Threading.Tasks;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Sales.Request;
using GuayaquilBank.Application.Dtos.Sales.Response;

namespace GuayaquilBank.Application.Contracts
{
    /// <summary>
    /// Define el contrato para el servicio de aplicación encargado de la gestión de clientes (Customers).
    /// Proporciona operaciones de lógica de negocio para el CRUD, paginación y control de estados,
    /// garantizando el aislamiento por Tenant (CompanyId) de forma transparente.
    /// </summary>
    public interface ICustomerAppService
    {
        /// <summary>
        /// Registra un nuevo cliente en el sistema asociado a la empresa (Tenant) del usuario en sesión.
        /// </summary>
        /// <param name="request">DTO que contiene los datos de identificación, perfil y contacto del cliente.</param>
        /// <returns>Un DTO con los datos del cliente recién creado, incluyendo su ID generado.</returns>
        /// <exception cref="GuayaquilBank.Domain.Exceptions.UserFriendlyException">
        /// Se lanza si la identificación (RUC/Cédula) ya se encuentra registrada en la misma empresa.
        /// </exception>
        Task<CustomerResponseDto> CreateAsync(CreateCustomerRequestDto request);

        /// <summary>
        /// Obtiene una lista paginada, filtrada y ordenada de los clientes pertenecientes al Tenant actual.
        /// </summary>
        /// <param name="request">Parámetros de paginación (índice, tamaño de página) y término opcional de búsqueda.</param>
        /// <returns>Un objeto de paginación que envuelve la colección de clientes que coinciden con los criterios.</returns>
        Task<PaginationResponseDto<CustomerResponseDto>> GetPagedAsync(PaginationRequestDto request);

        /// <summary>
        /// Recupera la información detallada de un cliente específico mediante su identificador único.
        /// </summary>
        /// <param name="customerId">Identificador único (GUID) del cliente.</param>
        /// <returns>El DTO del cliente si se encuentra y pertenece al Tenant; de lo contrario, <c>null</c>.</returns>
        Task<CustomerResponseDto?> GetByIdAsync(Guid customerId);

        /// <summary>
        /// Actualiza los datos de perfil e identificación de un cliente existente dentro del Tenant actual.
        /// </summary>
        /// <param name="customerId">Identificador único (GUID) del cliente a modificar.</param>
        /// <param name="request">DTO con los nuevos valores para actualizar el perfil del cliente.</param>
        /// <returns>El DTO del cliente con los cambios aplicados y persistidos.</returns>
        /// <exception cref="GuayaquilBank.Domain.Exceptions.UserFriendlyException">
        /// Se lanza si el cliente no existe o si la nueva identificación ingresada colisiona con otro cliente de la misma empresa.
        /// </exception>
        Task<CustomerResponseDto> UpdateAsync(Guid customerId, UpdateCustomerRequestDto request);

        /// <summary>
        /// Realiza un borrado lógico (Soft Delete) de un cliente en el sistema.
        /// </summary>
        /// <remarks>
        /// Al heredar la entidad de <c>FullAuditableEntity</c>, este método no elimina físicamente el registro,
        /// sino que activa la bandera de borrado para mantener la integridad referencial histórica en ventas y facturas.
        /// </remarks>
        /// <param name="customerId">Identificador único (GUID) del cliente a dar de baja de forma lógica.</param>
        /// <exception cref="GuayaquilBank.Domain.Exceptions.UserFriendlyException">
        /// Se lanza si el cliente solicitado no existe en los registros de la empresa.
        /// </exception>
        Task DeleteAsync(Guid customerId);

        /// <summary>
        /// Alterna de forma atómica el estado de actividad (Activo/Inactivo) de un cliente específico.
        /// </summary>
        /// <remarks>
        /// Permite suspender comercialmente a un cliente (por ejemplo, por problemas de crédito) sin necesidad de eliminarlo del sistema.
        /// </remarks>
        /// <param name="customerId">Identificador único (GUID) del cliente.</param>
        /// <exception cref="GuayaquilBank.Domain.Exceptions.UserFriendlyException">
        /// Se lanza si el cliente seleccionado no corresponde al entorno de la empresa actual.
        /// </exception>
        Task ToggleStatusAsync(Guid customerId);
    }
}