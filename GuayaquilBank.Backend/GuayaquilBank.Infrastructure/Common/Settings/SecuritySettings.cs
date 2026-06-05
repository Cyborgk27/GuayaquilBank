namespace GuayaquilBank.Infrastructure.Common.Settings
{
    public class SecuritySettings
    {
        public int HashWorkFactor { get; set; } = 12;
        public JwtSettings Jwt { get; set; } = new();
    }
}
