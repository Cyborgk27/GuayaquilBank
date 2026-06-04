using System;
using GuayaquilBank.Domain.Common;
using GuayaquilBank.Domain.Entities.Inventory;

namespace GuayaquilBank.Domain.Entities.Sales
{
    /// <summary>
    /// Representa un detalle específico de una factura dentro del sistema de ventas, 
    /// con propiedades esenciales como la referencia al lote de producto vendido, cantidad, 
    /// precio unitario, porcentaje de impuestos aplicados y cálculos automáticos de subtotal, 
    /// impuestos y total. Hereda de <see cref="FullAuditableEntity{Guid, Guid}"/> para incluir 
    /// funcionalidades de auditoría completa y borrado lógico. Esta clase es fundamental para 
    /// la gestión detallada de cada línea de venta en una factura, permitiendo un control preciso 
    /// sobre los productos vendidos y los mont
    /// </summary>
    public class InvoiceDetail : FullAuditableEntity<Guid, Guid>
    {
        public Guid InvoiceId { get; private set; }

        public Invoice Invoice { get; private set; } = null!;

        public Guid ProductBatchId { get; private set; }

        public ProductBatch ProductBatch { get; private set; } = null!;

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal TaxPercentage { get; private set; }

        public decimal SubTotal => Quantity * UnitPrice;

        public decimal TaxAmount => SubTotal * TaxPercentage;

        public decimal Total => SubTotal + TaxAmount;

        public InvoiceDetail(Guid id, Guid productBatchId, int quantity, decimal unitPrice, decimal taxPercentage)
            : base(id)
        {
            if (productBatchId == Guid.Empty)
                throw new ArgumentException("ProductBatchId cannot be an empty Guid.", nameof(productBatchId));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            if (unitPrice < 0.0m)
                throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));
            if (taxPercentage < 0.0m)
                throw new ArgumentException("Tax percentage cannot be negative.", nameof(taxPercentage));

            ProductBatchId = productBatchId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TaxPercentage = taxPercentage;
        }

#pragma warning disable CS8618
        private InvoiceDetail() : base() { }
#pragma warning restore CS8618

        internal void SetInvoice(Guid invoiceId)
        {
            if (invoiceId == Guid.Empty)
                throw new ArgumentException("InvoiceId cannot be an empty Guid.", nameof(invoiceId));
            InvoiceId = invoiceId;
        }
    }
}