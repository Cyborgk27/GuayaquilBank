namespace GuayaquilBank.Application.Dtos.Authentication.Response
{
    /// <summary>
    /// DTO que retorna los datos de sesión y el token de acceso tras un login exitoso.
    /// </summary>
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }
}
