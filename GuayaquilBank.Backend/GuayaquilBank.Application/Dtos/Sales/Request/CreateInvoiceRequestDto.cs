using System.ComponentModel.DataAnnotations;

namespace GuayaquilBank.Application.Dtos.Sales.Request
{
    public class CreateInvoiceRequestDto
    {
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public string ClientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de identificación (RUC/Cédula) es obligatorio.")]
        public string ClientIdentification { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "La factura debe contener al menos un detalle.")]
        public List<InvoiceDetailRequestDto> Details { get; set; } = new();
    }
}
