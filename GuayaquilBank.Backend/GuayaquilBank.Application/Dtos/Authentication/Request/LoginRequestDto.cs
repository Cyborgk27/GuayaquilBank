using System.ComponentModel.DataAnnotations;

namespace GuayaquilBank.Application.Dtos.Authentication.Request
{
    /// <summary>
    /// DTO encargado de recibir las credenciales de acceso para el inicio de sesión Multi-tenant.
    /// </summary>
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El dominio de la empresa es requerido.")]
        [StringLength(50, ErrorMessage = "El dominio no puede exceder los 50 caracteres.")]
        public string Domain { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [StringLength(50, ErrorMessage = "El usuario no puede exceder los 50 caracteres.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}