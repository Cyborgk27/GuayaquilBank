using GuayaquilBank.Domain.Common;
using GuayaquilBank.Domain.Entities.Identity;

namespace GuayaquilBank.Domain.Entities.Inventory
{
    /// <summary>
    /// Represents a product within the Guayaquil Bank inventory system, strictly isolated by company.
    /// Inherits full auditability and soft delete capabilities.
    /// </summary>
    public class Product : FullAuditableEntity<Guid, Guid>
    {
        private readonly List<ProductBatch> _batches = new();

        public Guid CompanyId { get; private set; }

        public Company Company { get; private set; } = null!;

        public string Sku { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public int MinimumStockAlert { get; private set; }

        public IReadOnlyCollection<ProductBatch> Batches => _batches.AsReadOnly();

        public bool IsActive { get; private set; }

        public Product(Guid id, Guid companyId, string sku, string name, string description, int minimumStockAlert = 0)
            : base(id)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("CompanyId cannot be an empty Guid.", nameof(companyId));

            CompanyId = companyId;
            Sku = ThrowIfNullOrEmpty(sku, nameof(sku)).Trim().ToUpperInvariant();
            Name = ThrowIfNullOrEmpty(name, nameof(name));
            Description = description;

            if (minimumStockAlert < 0)
                throw new ArgumentException("Minimum stock alert cannot be negative.", nameof(minimumStockAlert));

            MinimumStockAlert = minimumStockAlert;
            IsActive = true;
        }

#pragma warning disable CS8618
        private Product() : base() { }
#pragma warning restore CS8618

        // --- Domain Business Behaviors (DDD) ---

        public void UpdateDetails(string name, string description, int minimumStockAlert)
        {
            if (minimumStockAlert < 0)
                throw new ArgumentException("Minimum stock alert cannot be negative.", nameof(minimumStockAlert));

            Name = ThrowIfNullOrEmpty(name, nameof(name));
            Description = description;
            MinimumStockAlert = minimumStockAlert;
        }

        public void UpdateSku(string newSku)
        {
            Sku = ThrowIfNullOrEmpty(newSku, nameof(newSku)).Trim().ToUpperInvariant();
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        private static string ThrowIfNullOrEmpty(string value, string paramName)
        {
            return string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName)
                : value;
        }
    }
}