using System.Threading.Tasks;
using GuayaquilBank.Application.Dtos.Profile.Request;
using GuayaquilBank.Application.Dtos.Profile.Response;

namespace GuayaquilBank.Application.Contracts
{
    /// <summary>
    /// Contrato para el servicio de aplicación encargado de la autogestión del perfil del usuario 
    /// actualmente autenticado y de la configuración comercial de su empresa (Tenant).
    /// </summary>
    public interface IProfileAppService
    {
        /// <summary>
        /// Obtiene el perfil detallado del usuario autenticado en la sesión actual.
        /// </summary>
        /// <returns>Un DTO con los datos de cuenta y perfil del operador.</returns>
        Task<MyUserResponseDto> GetMyProfileAsync();

        /// <summary>
        /// Actualiza la información de perfil (correo electrónico, nombres, apellidos y avatar) del usuario actual.
        /// </summary>
        /// <param name="request">DTO con los nuevos valores de perfil validados.</param>
        /// <returns>El perfil actualizado reflejando los cambios confirmados.</returns>
        Task<MyUserResponseDto> UpdateMyProfileAsync(UpdateMyProfileRequestDto request);

        /// <summary>
        /// Obtiene la información comercial y de contacto de la empresa (Tenant) a la que pertenece el usuario.
        /// </summary>
        /// <returns>Un DTO con los datos de la organización/empresa.</returns>
        Task<MyCompanyResponseDto> GetMyCompanyAsync();

        /// <summary>
        /// Actualiza los datos corporativos y de facturación de la empresa actual (Razón social, RUC, Dirección, etc.).
        /// </summary>
        /// <param name="request">DTO con los datos de la empresa actualizados.</param>
        /// <returns>La información de la empresa con los cambios persistidos.</returns>
        Task<MyCompanyResponseDto> UpdateMyCompanyAsync(UpdateMyCompanyRequestDto request);
    }
}