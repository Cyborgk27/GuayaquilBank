using GuayaquilBank.Domain.Entities.Identity;

namespace GuayaquilBank.Domain.Interfaces
{
    /// <summary>
    /// Proporciona una abstracción para generar tokens de seguridad de autenticación.
    /// </summary>
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
