using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Sales.Request;
using GuayaquilBank.Application.Dtos.Sales.Response;

namespace GuayaquilBank.Application.Contracts
{
    /// <summary>
    /// Define las operaciones de negocio para la generación de ventas, control de facturación
    /// y emisión de comprobantes en formato PDF para el banco.
    /// </summary>
    public interface ISalesAppService
    {
        /// <summary>
        /// Registra una nueva venta, calcula los totales correspondientes de forma interna 
        /// y deduce el stock de los lotes involucrados de manera atómica.
        /// </summary>
        Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceRequestDto request);

        /// <summary>
        /// Obtiene el listado histórico de facturas de la compañía actual de manera paginado.
        /// </summary>
        Task<PaginationResponseDto<InvoiceResponseDto>> GetPagedInvoicesAsync(PaginationRequestDto request);

        /// <summary>
        /// Obtiene los detalles de una factura específica por su ID único.
        /// </summary>
        Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid invoiceId);

        /// <summary>
        /// Genera el documento de impresión físico de la factura en formato binario PDF.
        /// </summary>
        /// <param name="invoiceId">ID único de la factura a procesar.</param>
        /// <returns>Arreglo de bytes listo para ser descargado o visualizado en Angular.</returns>
        Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId);
    }
}