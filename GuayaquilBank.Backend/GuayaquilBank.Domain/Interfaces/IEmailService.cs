namespace GuayaquilBank.Domain.Interfaces
{
    /// <summary>
    /// Proporciona una abstracción para enviar notificaciones por correo electrónico transaccionales.
    /// </summary>
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
