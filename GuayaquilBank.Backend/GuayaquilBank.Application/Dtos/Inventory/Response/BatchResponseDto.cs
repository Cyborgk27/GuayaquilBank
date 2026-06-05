using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuayaquilBank.Application.Dtos.Inventory.Response
{
    /// <summary>
    /// Representación plana de un lote de inventario vinculada al producto.
    /// </summary>
    public class BatchResponseDto
    {
        public Guid Id { get; set; }
        public decimal UnitCost { get; set; }
        public int CurrentQuantity { get; set; }
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Indica si el lote ya no puede ser comercializado.
        /// </summary>
        public bool IsExpired => ExpirationDate.HasValue && ExpirationDate.Value.Date < DateTime.UtcNow.Date;
    }
}
