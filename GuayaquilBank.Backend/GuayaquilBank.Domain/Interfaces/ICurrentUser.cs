namespace GuayaquilBank.Domain.Interfaces
{
    /// <summary>
    /// Provee una abstracción para acceder a la información del usuario actual en el contexto de la aplicación,
    /// </summary>
    public interface ICurrentUser
    {
        Guid? UserId { get; }

        Guid? CompanyId { get; }

        bool IsAuthenticated { get; }
    }
}