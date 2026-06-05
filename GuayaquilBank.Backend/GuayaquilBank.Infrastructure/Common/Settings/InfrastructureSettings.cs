namespace GuayaquilBank.Infrastructure.Common.Settings
{
    public class InfrastructureSettings
    {
        public EmailSettings Email { get; set; } = new();
        public string DatabaseProvider { get; set; } = null!;
        public SecuritySettings Security { get; set; } = new();
        public CorsSettings Cors { get; set; } = new();
    }
}
