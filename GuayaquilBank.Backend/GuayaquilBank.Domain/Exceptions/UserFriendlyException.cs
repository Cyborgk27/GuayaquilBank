namespace GuayaquilBank.Domain.Exceptions
{
    /// <summary>
    /// Indica un error de lógica de negocio o de validación con un mensaje seguro que se 
    /// puede mostrar directamente al usuario final.
    /// </summary>
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException(string message) : base(message)
        {
        }
        public UserFriendlyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
