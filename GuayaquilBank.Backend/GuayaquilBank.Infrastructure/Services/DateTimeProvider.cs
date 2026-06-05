using GuayaquilBank.Domain.Interfaces;

namespace GuayaquilBank.Infrastructure.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
