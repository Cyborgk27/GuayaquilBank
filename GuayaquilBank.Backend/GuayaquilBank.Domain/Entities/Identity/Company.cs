using GuayaquilBank.Domain.Common;

namespace GuayaquilBank.Domain.Entities.Identity
{
    /// <summary>
    /// Representa a una empresa o entidad corporativa dentro del sistema, 
    /// con propiedades esenciales como nombre, identificación fiscal, 
    /// dominio web y estado de actividad. Hereda de <see cref="FullAuditableEntity{Guid, Guid}"/> 
    /// para incluir funcionalidades de auditoría completa y borrado lógico. Esta clase es fundamental 
    /// para la gestión de múltiples empresas o clientes corporativos en el sistema, permitiendo la 
    /// segregación de datos y configuraciones específicas por empresa.
    /// </summary>
    public class Company : FullAuditableEntity<Guid, Guid>
    {
        public string Name { get; private set; }

        public string TaxId { get; private set; }

        public string Email { get; private set; }

        public string PhoneNumber { get; private set; }

        public string CurrencySymbol { get; private set; }

        public decimal Iva { get; private set; }

        public string Address { get; private set; }

        public string City { get; private set; }

        public string Region { get; private set; }

        public string PostalCode { get; private set; }

        public string Country { get; private set; }

        public string Domain { get; private set; }

        public string? LogoUrl { get; private set; }

        public bool IsActive { get; private set; }

        public Company(
            Guid id,
            string name,
            string taxId,
            string domain,
            string email,
            string phoneNumber,
            string address,
            string city,
            string region,
            string postalCode,
            string country,
            string? logoUrl = null,
            string currencySymbol = "$",
            decimal iva = 0.0m)
            : base(id)
        {
            Name = ThrowIfNullOrEmpty(name, nameof(name));
            TaxId = ThrowIfNullOrEmpty(taxId, nameof(taxId));
            Domain = ThrowIfNullOrEmpty(domain, nameof(domain));
            Email = ThrowIfNullOrEmpty(email, nameof(email));
            PhoneNumber = ThrowIfNullOrEmpty(phoneNumber, nameof(phoneNumber));
            Address = ThrowIfNullOrEmpty(address, nameof(address));
            City = ThrowIfNullOrEmpty(city, nameof(city));
            Region = ThrowIfNullOrEmpty(region, nameof(region));
            PostalCode = ThrowIfNullOrEmpty(postalCode, nameof(postalCode));
            Country = ThrowIfNullOrEmpty(country, nameof(country));

            if (iva < 0.0m)
                throw new ArgumentException("Tax rate (IVA) cannot be negative.", nameof(iva));

            LogoUrl = logoUrl;
            CurrencySymbol = ThrowIfNullOrEmpty(currencySymbol, nameof(currencySymbol));
            Iva = iva;
            IsActive = true;
        }

#pragma warning disable CS8618
        private Company() : base() { }
#pragma warning restore CS8618

        // --- Domain Business Behaviors (DDD) ---

        public void UpdateProfile(string newName, string newDomain, string? newLogoUrl)
        {
            Name = ThrowIfNullOrEmpty(newName, nameof(newName));
            Domain = ThrowIfNullOrEmpty(newDomain, nameof(newDomain));
            LogoUrl = newLogoUrl;
        }

        public void UpdateTaxId(string newTaxId)
        {
            TaxId = ThrowIfNullOrEmpty(newTaxId, nameof(newTaxId));
        }

        public void UpdateContactInfo(string newEmail, string newPhoneNumber)
        {
            Email = ThrowIfNullOrEmpty(newEmail, nameof(newEmail));
            PhoneNumber = ThrowIfNullOrEmpty(newPhoneNumber, nameof(newPhoneNumber));
        }

        public void UpdateAddress(string newAddress, string newCity, string newRegion, string newPostalCode, string newCountry)
        {
            Address = ThrowIfNullOrEmpty(newAddress, nameof(newAddress));
            City = ThrowIfNullOrEmpty(newCity, nameof(newCity));
            Region = ThrowIfNullOrEmpty(newRegion, nameof(newRegion));
            PostalCode = ThrowIfNullOrEmpty(newPostalCode, nameof(newPostalCode));
            Country = ThrowIfNullOrEmpty(newCountry, nameof(newCountry));
        }

        public void ConfigureFinancialSettings(string newCurrencySymbol, decimal newIva)
        {
            if (newIva < 0.0m)
                throw new ArgumentException("Tax rate (IVA) cannot be negative.", nameof(newIva));

            CurrencySymbol = ThrowIfNullOrEmpty(newCurrencySymbol, nameof(newCurrencySymbol));
            Iva = newIva;
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