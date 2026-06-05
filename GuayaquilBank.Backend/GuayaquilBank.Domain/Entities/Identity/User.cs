using System;
using GuayaquilBank.Domain.Common;

namespace GuayaquilBank.Domain.Entities.Identity
{
    /// <summary>
    /// Representa a un usuario del sistema, con propiedades esenciales como nombre de usuario, 
    /// correo electrónico, nombre y apellido, y estado de actividad. Hereda de <see cref="FullAuditableEntity{Guid, Guid}"/> para incluir funcionalidades de auditoría completa y borrado lógico.
    /// </summary>
    public class User : FullAuditableEntity<Guid, Guid>
    {
        public Guid CompanyId { get; private set; }

        public Company Company { get; private set; } = null!;

        public string Username { get; private set; }
        public string PasswordHash { get; private set; }

        public string Email { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string FullName => $"{FirstName} {LastName}".Trim();

        public string ProfilePictureUrl { get; private set; }

        public bool IsActive { get; private set; }

        public User(Guid id, Guid companyId, string username, string passwordHash, string email, string firstName, string lastName, string profilePictureUrl = "")
            : base(id)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("CompanyId cannot be an empty Guid.", nameof(companyId));

            CompanyId = companyId;
            Username = ThrowIfNullOrEmpty(username, nameof(username));
            PasswordHash = ThrowIfNullOrEmpty(passwordHash, nameof(passwordHash));
            Email = ThrowIfNullOrEmpty(email, nameof(email));
            FirstName = ThrowIfNullOrEmpty(firstName, nameof(firstName));
            LastName = ThrowIfNullOrEmpty(lastName, nameof(lastName));
            ProfilePictureUrl = profilePictureUrl;
            IsActive = true;
        }

#pragma warning disable CS8618
        private User() : base() { }
#pragma warning restore CS8618

        public void ChangeCompany(Guid newCompanyId)
        {
            if (newCompanyId == Guid.Empty)
                throw new ArgumentException("The new CompanyId cannot be an empty Guid.", nameof(newCompanyId));

            CompanyId = newCompanyId;
        }

        public void UpdateProfilePicture(string newProfilePictureUrl)
        {
            ProfilePictureUrl = newProfilePictureUrl;
        }

        public void ChangeEmail(string newEmail)
        {
            Email = ThrowIfNullOrEmpty(newEmail, nameof(newEmail));
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