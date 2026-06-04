using GuayaquilBank.Domain.Common;

namespace GuayaquilBank.Domain.Entities.Inventory
{
    /// <summary>
    /// Representa un lote o partida específica de un producto en el sistema de inventario, 
    /// con propiedades esenciales como número de lote, cantidad disponible, costo unitario, 
    /// fechas de fabricación y vencimiento. Esta clase es fundamental para la gestión detallada 
    /// del inventario, permitiendo rastrear cada lote individualmente para propósitos de control 
    /// de calidad, rotación de stock y cumplimiento normativo.
    /// </summary>
    public class ProductBatch : FullAuditableEntity<Guid, Guid>
    {
        public Guid ProductId { get; private set; }

        public Product Product { get; private set; } = null!;

        public string BatchNumber { get; private set; }

        public int Quantity { get; private set; }

        public decimal UnitCost { get; private set; }

        public DateTime ManufacturedAt { get; private set; }

        public DateTime? ExpirationDate { get; private set; }

        public ProductBatch(
            Guid id,
            Guid productId,
            string batchNumber,
            int quantity,
            decimal unitCost,
            DateTime manufacturedAt,
            DateTime? expirationDate = null)
            : base(id)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be an empty Guid.", nameof(productId));
            if (quantity < 0)
                throw new ArgumentException("Initial quantity cannot be negative.", nameof(quantity));
            if (unitCost < 0.0m)
                throw new ArgumentException("Unit cost cannot be negative.", nameof(unitCost));
            if (expirationDate.HasValue && expirationDate.Value <= manufacturedAt)
                throw new ArgumentException("Expiration date must be later than the manufacturing date.", nameof(expirationDate));

            ProductId = productId;
            BatchNumber = ThrowIfNullOrEmpty(batchNumber, nameof(batchNumber)).Trim().ToUpperInvariant();
            Quantity = quantity;
            UnitCost = unitCost;
            ManufacturedAt = manufacturedAt;
            ExpirationDate = expirationDate;
        }

#pragma warning disable CS8618
        private ProductBatch() : base() { }
#pragma warning restore CS8618

        // --- Domain Business Behaviors (DDD) ---

        public void AddStock(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount to add must be greater than zero.", nameof(amount));

            Quantity += amount;
        }

        public void DeductStock(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount to deduct must be greater than zero.", nameof(amount));
            if (Quantity - amount < 0)
                throw new ArgumentException("Insufficient stock in this batch to complete the deduction.", nameof(amount));

            Quantity -= amount;
        }

        public bool IsExpired(DateTime currentDate)
        {
            return ExpirationDate.HasValue && ExpirationDate.Value.Date <= currentDate.Date;
        }

        private static string ThrowIfNullOrEmpty(string value, string paramName)
        {
            return string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName)
                : value;
        }
    }
}