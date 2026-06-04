using GuayaquilBank.Domain.Common;
using GuayaquilBank.Domain.Entities.Identity;

namespace GuayaquilBank.Domain.Entities.Sales
{
    /// <summary>
    /// Representa a un cliente dentro del sistema de ventas, con propiedades esenciales como 
    /// identificación legal, nombre completo, correo electrónico, número de teléfono, dirección 
    /// y estado de actividad. Hereda de <see cref="FullAuditableEntity{Guid, Guid}"/> para incluir 
    /// funcionalidades de auditoría completa y borrado lógico. Esta clase es fundamental para la 
    /// gestión de clientes en el sistema, permitiendo la asociación de cada cliente a una empresa 
    /// específica y facilitando las operaciones comerciales relacionadas con ventas e facturación.
    /// </summary>
    public class Customer : FullAuditableEntity<Guid, Guid>
    {
        public Guid CompanyId { get; private set; }

        public Company Company { get; private set; } = null!;

        public string Identification { get; private set; }

        public string FullName { get; private set; }

        public string Email { get; private set; }

        public string PhoneNumber { get; private set; }

        public string Address { get; private set; }

        public bool IsActive { get; private set; }


        public Customer(
            Guid id,
            Guid companyId,
            string identification,
            string fullName,
            string email,
            string phoneNumber,
            string address)
            : base(id)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("CompanyId cannot be an empty Guid.", nameof(companyId));

            CompanyId = companyId;
            Identification = ThrowIfNullOrEmpty(identification, nameof(identification)).Trim();
            FullName = ThrowIfNullOrEmpty(fullName, nameof(fullName));
            Email = ThrowIfNullOrEmpty(email, nameof(email));
            PhoneNumber = phoneNumber?.Trim() ?? string.Empty;
            Address = address?.Trim() ?? string.Empty;
            IsActive = true;
        }

#pragma warning disable CS8618
        private Customer() : base() { }
#pragma warning restore CS8618

        // --- Domain Business Behaviors (DDD) ---

        public void UpdateProfile(string newFullName, string newEmail, string newPhoneNumber, string newAddress)
        {
            FullName = ThrowIfNullOrEmpty(newFullName, nameof(newFullName));
            Email = ThrowIfNullOrEmpty(newEmail, nameof(newEmail));
            PhoneNumber = newPhoneNumber?.Trim() ?? string.Empty;
            Address = newAddress?.Trim() ?? string.Empty;
        }

        public void UpdateIdentification(string newIdentification)
        {
            Identification = ThrowIfNullOrEmpty(newIdentification, nameof(newIdentification)).Trim();
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        private static string ThrowIfNullOrEmpty(string value, string paramName)
        {
            return string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName)
                : value;
        }
    }
}