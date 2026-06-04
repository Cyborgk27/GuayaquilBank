using GuayaquilBank.Domain.Common;
using GuayaquilBank.Domain.Entities.Identity;

namespace GuayaquilBank.Domain.Entities.Sales
{
    /// <summary>
    /// Representa una factura dentro del sistema de ventas, con propiedades esenciales como la referencia 
    /// a la empresa emisora, el cliente comprador, el número de factura, la fecha de emisión y una colección 
    /// de detalles de factura. Hereda de <see cref="FullAuditableEntity{Guid, Guid}"/> para incluir 
    /// funcionalidades de auditoría completa y borrado lógico. Esta clase es fundamental para la gestión de 
    /// facturas en el sistema, permitiendo un control preciso sobre las transacciones comerciales y 
    /// facilitando la generación de reportes financieros y fiscales. Además, incluye comportamientos de dominio 
    /// para agregar detalles a la factura y calcular totales dinámicamente.
    /// </summary>
    public class Invoice : FullAuditableEntity<Guid, Guid>
    {
        private readonly List<InvoiceDetail> _details = new();

        public Guid CompanyId { get; private set; }

        public Company Company { get; private set; } = null!;

        public Guid CustomerId { get; private set; }

        public Customer Customer { get; private set; } = null!;

        public string InvoiceNumber { get; private set; }

        public DateTime IssuedAtUtc { get; private set; }

        public IReadOnlyCollection<InvoiceDetail> Details => _details.AsReadOnly();

        public decimal SubTotal => _details.Sum(d => d.SubTotal);

        public decimal TotalTax => _details.Sum(d => d.TaxAmount);

        public decimal Total => _details.Sum(d => d.Total);

        public Invoice(Guid id, Guid companyId, Guid customerId, string invoiceNumber, DateTime issuedAtUtc)
            : base(id)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("CompanyId cannot be an empty Guid.", nameof(companyId));
            if (customerId == Guid.Empty)
                throw new ArgumentException("CustomerId cannot be an empty Guid.", nameof(customerId));

            CompanyId = companyId;
            CustomerId = customerId;
            InvoiceNumber = ThrowIfNullOrEmpty(invoiceNumber, nameof(invoiceNumber)).Trim();
            IssuedAtUtc = issuedAtUtc;
        }

#pragma warning disable CS8618
        private Invoice() : base() { }
#pragma warning restore CS8618

        // --- Domain Business Behaviors (DDD Aggregate Root Control) ---
        public void AddItem(Guid detailId, Guid productBatchId, int quantity, decimal unitPrice, decimal taxPercentage)
        {
            if (_details.Any(d => d.ProductBatchId == productBatchId))
                throw new InvalidOperationException("This product batch has already been added to the invoice details.");

            var detail = new InvoiceDetail(detailId, productBatchId, quantity, unitPrice, taxPercentage);
            detail.SetInvoice(Id);

            _details.Add(detail);
        }

        private static string ThrowIfNullOrEmpty(string value, string paramName)
        {
            return string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName)
                : value;
        }
    }
}