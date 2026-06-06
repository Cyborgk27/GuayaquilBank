using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Sales.Request;
using GuayaquilBank.Application.Dtos.Sales.Response;
using GuayaquilBank.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuayaquilBank.WebApi.Controllers
{
    /// <summary>
    /// Controlador encargado de la emisión de facturas, control de ventas y generación de comprobantes PDF.
    /// Garantiza el aislamiento transaccional por Tenant de forma nativa.
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    public class SalesController : BaseApiController
    {
        private readonly ISalesAppService _appService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="SalesController"/>.
        /// </summary>
        /// <param name="appService">Servicio de aplicación encargado de la lógica transaccional de ventas.</param>
        public SalesController(ISalesAppService appService)
        {
            _appService = appService;
        }

        /// <summary>
        /// Crea una nueva factura y descuenta el stock de los lotes correspondientes de forma atómica.
        /// </summary>
        /// <remarks>
        /// Este proceso se ejecuta dentro de una transacción de base de datos para asegurar que si el descuento de stock falla, 
        /// la factura no se emita.
        /// </remarks>
        /// <param name="request">DTO con los datos del cliente, desglose de ítems, cantidades y formas de pago.</param>
        /// <returns>La estructura de la factura emitida con sus totales, IVA y secuencial calculado.</returns>
        /// <response code="201">Factura procesada y emitida con éxito.</response>
        /// <response code="400">Si no hay stock suficiente en los lotes o los totales del cliente no cuadran matemáticamente.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<InvoiceResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateInvoiceRequestDto request)
        {
            var result = await _appService.CreateInvoiceAsync(request);

            var response = ApiResponse<InvoiceResponseDto>.SuccessResponse(result, "Factura emitida y procesada correctamente.", StatusCodes.Status200OK);
            return CreatedAtAction("GetById", new { id = result.Id }, response);
        }

        /// <summary>
        /// Obtiene el historial de facturas emitidas de forma paginada y filtrada por cliente, RUC o secuencia.
        /// </summary>
        /// <param name="request">Parámetros de paginación y criterios de búsqueda o filtrado por fechas.</param>
        /// <returns>Colección estructurada de comprobantes que pertenecen al Tenant del usuario.</returns>
        /// <response code="200">Retorna el listado histórico de facturación envuelto en la estructura global de éxito.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponseDto<InvoiceResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedAsync([FromQuery] PaginationRequestDto request)
        {
            var result = await _appService.GetPagedInvoicesAsync(request);

            return ToResponse(result, "Historial de facturación recuperado con éxito.");
        }

        /// <summary>
        /// Obtiene el detalle general y desglose de ítems de una factura específica por su identificador único.
        /// </summary>
        /// <param name="id">Identificador único (GUID) de la factura.</param>
        /// <returns>Ficha detallada con los ítems, precios, impuestos y totales calculados de la venta.</returns>
        /// <response code="200">Retorna la factura localizada.</response>
        /// <response code="404">Si el documento no existe o pertenece a otra entidad comercial.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<InvoiceResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _appService.GetInvoiceByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse($"La factura con ID {id} no fue localizada en este entorno.", null, StatusCodes.Status404NotFound));
            }

            return ToResponse(result, "Detalle de factura recuperado de forma satisfactoria.");
        }

        /// <summary>
        /// Genera y descarga el documento físico PDF profesional de la factura usando QuestPDF.
        /// </summary>
        /// <remarks>
        /// Este endpoint retorna un flujo binario directo (application/pdf). No se encapsula en ApiResponse ya que está diseñado para descarga nativa.
        /// </remarks>
        /// <param name="id">Identificador único (GUID) de la factura de la cual se desea el PDF.</param>
        /// <returns>Archivo binario de extensión .pdf listo para su renderizado o impresión.</returns>
        /// <response code="200">Retorna el archivo binario listo para descarga.</response>
        /// <response code="404">Si la factura solicitada no existe.</response>
        [HttpGet("{id:guid}/pdf")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadPdfAsync(Guid id)
        {
            byte[] pdfBytes = await _appService.GenerateInvoicePdfAsync(id);

            string fileName = $"Factura_{id}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}