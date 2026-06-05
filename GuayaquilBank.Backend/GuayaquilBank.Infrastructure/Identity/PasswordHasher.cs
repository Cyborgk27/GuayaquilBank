using GuayaquilBank.Domain.Interfaces;
using GuayaquilBank.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;
using BC = BCrypt.Net.BCrypt;

namespace GuayaquilBank.Infrastructure.Identity
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly int _workFactor;
        public PasswordHasher(IOptions<InfrastructureSettings> settings)
        {
            _workFactor = settings.Value.Security.HashWorkFactor;
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
