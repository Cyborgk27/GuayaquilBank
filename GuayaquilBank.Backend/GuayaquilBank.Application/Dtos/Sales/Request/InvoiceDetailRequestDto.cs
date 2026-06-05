using System.ComponentModel.DataAnnotations;

namespace GuayaquilBank.Application.Dtos.Sales.Request
{
    public class InvoiceDetailRequestDto
    {
        [Required(ErrorMessage = "El producto es obligatorio.")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "El lote específico es obligatorio para mantener la trazabilidad.")]
        public Guid ProductBatchId { get; set; }

        [Range(1, 100000, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Quantity { get; set; }

        [Range(0.00, 999999.99, ErrorMessage = "El precio unitario no puede ser negativo.")]
        public decimal UnitPrice { get; set; }
    }
}
