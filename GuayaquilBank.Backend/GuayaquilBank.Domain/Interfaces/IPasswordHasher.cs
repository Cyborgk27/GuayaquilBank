namespace GuayaquilBank.Domain.Interfaces
{
    /// <summary>
    /// Proporciona una abstracción para el hash y la verificación de contraseñas de usuario de forma segura.
    /// </summary>
    public interface IPasswordHasher
    {
        string HashPassword(string password);

        bool VerifyPassword(string password, string hashedPassword);
    }
}
