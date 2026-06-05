using System.ComponentModel.DataAnnotations;

namespace GuayaquilBank.Application.Dtos.Inventory.Request
{
    public class CreateProductRequestDto
    {
        [Required(ErrorMessage = "El nombre del producto es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El SKU o código de barra es requerido.")]
        [StringLength(50, ErrorMessage = "El SKU no puede superar los 50 caracteres.")]
        public string Sku { get; set; } = string.Empty;

        public string? Description { get; set; }

        public CreateProductBatchRequestDto? InitialBatch { get; set; }
    }
}