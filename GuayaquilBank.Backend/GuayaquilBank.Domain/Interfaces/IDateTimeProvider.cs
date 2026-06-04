namespace GuayaquilBank.Domain.Interfaces
{
    /// <summary>
    /// Proporciona una abstracción para obtener la fecha y la hora actuales del sistema.
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
