using GuayaquilBank.Domain.Interfaces;
using GuayaquilBank.Infrastructure.Common.Settings;
using BC = BCrypt.Net.BCrypt;

namespace GuayaquilBank.Infrastructure.Identity
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly int _workFactor;
        public PasswordHasher(InfrastructureSettings settings)
        {
            _workFactor = settings.Security.HashWorkFactor;
        }
        public string HashPassword(string password)
        {
            return BC.HashPassword(password, _workFactor);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BC.Verify(password, hashedPassword);
        }
    }
}
