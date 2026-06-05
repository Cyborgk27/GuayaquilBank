using GuayaquilBank.Application.Dtos.Authentication.Request;
using GuayaquilBank.Application.Dtos.Authentication.Response;

namespace GuayaquilBank.Application.Contracts
{
    /// <summary>
    /// Define los casos de uso del módulo de identidad y control de accesos multi-tenant del banco.
    /// </summary>
    public interface IAuthenticationAppService
    {
        /// <summary>
        /// Valida las credenciales de un usuario contra el dominio de su empresa y genera una sesión activa.
        /// </summary>
        /// <param name="request">DTO con el dominio, nombre de usuario y contraseña.</param>
        /// <returns>Los datos del perfil del usuario junto con su Token JWT de acceso.</returns>
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}