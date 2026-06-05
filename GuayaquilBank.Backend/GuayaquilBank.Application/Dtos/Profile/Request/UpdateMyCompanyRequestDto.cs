using System.ComponentModel.DataAnnotations;

namespace GuayaquilBank.Application.Dtos.Profile.Request
{
    public class UpdateMyCompanyRequestDto
    {
        [Required(ErrorMessage = "El nombre de la empresa es obligatorio.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La identificación fiscal (RUC/TaxId) es obligatoria.")]
        public string TaxId { get; set; } = null!;

        [Required(ErrorMessage = "El dominio web es obligatorio.")]
        public string Domain { get; set; } = null!;

        [Required(ErrorMessage = "El correo de la empresa es obligatorio.")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "El teléfono de contacto es obligatorio.")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "La dirección de la matriz es obligatoria.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "La provincia o región es obligatoria.")]
        public string Region { get; set; } = null!;

        [Required(ErrorMessage = "El código postal es obligatorio.")]
        public string PostalCode { get; set; } = null!;

        [Required(ErrorMessage = "El país es obligatorio.")]
        public string Country { get; set; } = null!;

        public string? LogoUrl { get; set; }

        [Required(ErrorMessage = "El símbolo de moneda es obligatorio.")]
        public string CurrencySymbol { get; set; } = "$";

        [Required(ErrorMessage = "El porcentaje de IVA es obligatorio.")]
        [Range(0.0, 100.0, ErrorMessage = "El IVA no puede ser un valor negativo.")]
        public decimal Iva { get; set; }
    }
}