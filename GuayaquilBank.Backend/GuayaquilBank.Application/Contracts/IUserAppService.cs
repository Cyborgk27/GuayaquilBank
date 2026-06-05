using System;
using System.Threading.Tasks;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Identity.Request;
using GuayaquilBank.Application.Dtos.Identity.Response;

namespace GuayaquilBank.Application.Contracts
{
    /// <summary>
    /// Define el contrato para el servicio de aplicación encargado del control de accesos, 
    /// gestión de identidad y administración de usuarios (Users).
    /// Asegura el aislamiento por Tenant (CompanyId) en las consultas y operaciones mutables.
    /// </summary>
    public interface IUserAppService
    {
        /// <summary>
        /// Registra un nuevo operador o usuario en el sistema, vinculándolo al Tenant actual y encriptando sus credenciales.
        /// </summary>
        /// <param name="request">DTO con los datos de perfil, correo electrónico y la contraseña en texto plano.</param>
        /// <returns>Un DTO con la información pública del usuario creado (excluyendo datos sensibles de contraseñas).</returns>
        /// <exception cref="GuayaquilBank.Domain.Exceptions.UserFriendlyException">
        /// Se lanza si el Username o el Email ya se encuentran registrados de forma global o dentro del mismo entorno comercial.
        /// </exception>
        Task<UserResponseDto> CreateAsync(CreateUserRequestDto request);

        /// <summary>
        /// Obtiene un listado paginado, filtrado y ordenado de todos los usuarios vinculados exclusivamente a la empresa del operador en sesión.
        /// </summary>
        /// <param name="request">Parámetros de paginación (Page, PageSize) y términos opcionales de búsqueda para nombres, usuario o email.</param>
        /// <returns>Un objeto contenedor de paginación con la lista de usuarios mapeada a DTOs seguros.</returns>
        Task<PaginationResponseDto<UserResponseDto>> GetPagedAsync(PaginationRequestDto request);

        /// <summary>
        /// Recupera el perfil detallado de un usuario mediante su identificador único (GUID), validando su pertenencia al Tenant actual.
        /// </summary>
        /// <param name="userId">Identificador único (GUID) del usuario a consultar.</param>
        /// <returns>El DTO del usuario localizado; o bien, <c>null</c> si el usuario no existe o pertenece a otro Tenant.</returns>
        Task<UserResponseDto?> GetByIdAsync(Guid userId);

        /// <summary>
        /// Actualiza la información de perfil, nombres, apellidos y correo electrónico de un usuario operativo existente.
        /// </summary>
        /// <param name="userId">Identificador único (GUID) del usuario a modificar.</param>
        /// <param name="request">DTO con la estructura de datos actualizada y validada para el perfil.</param>
        /// <returns>El DTO del usuario reflejando los cambios confirmados y guardados en la base de datos.</returns>
        /// <exception cref="GuayaquilBank.Domain.Exceptions.UserFriendlyException">
        /// Se lanza si el usuario solicitado no existe dentro del Tenant, o si el nuevo correo ingresado ya está ocupado por otra cuenta.
        /// </exception>
        Task<UserResponseDto> UpdateAsync(Guid userId, UpdateUserRequestDto request);

        /// <summary>
        /// Ejecuta una baja lógica (Soft Delete) del usuario especificado en el sistema.
        /// </summary>
        /// <remarks>
        /// Al heredar de <c>FullAuditableEntity</c>, el registro no se elimina físicamente del disco para preservar 
        /// la trazabilidad histórica de auditorías (quién creó facturas, quién registró clientes, etc.), sino que se activa su bandera de borrado.
        /// </remarks>
        /// <param name="userId">Identificador único (GUID) del usuario a eliminar lógicamente.</param>
        /// <exception cref="GuayaquilBank.Domain.Exceptions.UserFriendlyException">
        /// Se lanza si el usuario no pertenece a la organización actual o no fue localizado.
        /// </exception>
        Task DeleteAsync(Guid userId);

        /// <summary>
        /// Conmuta de forma atómica el estado de actividad (Activo/Inactivo) de un usuario en el sistema.
        /// </summary>
        /// <remarks>
        /// Permite revocar o restaurar los accesos de un operador a la plataforma ERP de forma instantánea sin destruir su cuenta ni sus logs.
        /// </remarks>
        /// <param name="userId">Identificador único (GUID) del usuario cuyo estado se desea alternar.</param>
        /// <exception cref="GuayaquilBank.Domain.Exceptions.UserFriendlyException">
        /// Se lanza si el identificador no coincide con un usuario válido dentro del alcance del Tenant actual.
        /// </exception>
        Task ToggleStatusAsync(Guid userId);
    }
}