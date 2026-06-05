using System;
using System.ComponentModel.DataAnnotations;

namespace GuayaquilBank.Application.Dtos.Inventory.Request
{
    public class CreateProductBatchRequestDto
    {
        [Required(ErrorMessage = "El número de lote es requerido.")]
        [StringLength(50, ErrorMessage = "El número de lote no puede superar los 50 caracteres.")]
        public string BatchNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cantidad es requerida.")]
        [Range(1, 100000, ErrorMessage = "La cantidad debe ser de al menos 1 unidad.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "El costo unitario es requerido.")]
        [Range(0.01, 999999.99, ErrorMessage = "El costo debe ser mayor a cero.")]
        public decimal UnitCost { get; set; }

        [Required(ErrorMessage = "La fecha de fabricación es requerida.")]
        public DateTime ManufacturedAt { get; set; }

        public DateTime? ExpirationDate { get; set; }
    }
}