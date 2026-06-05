using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Sales.Request;
using GuayaquilBank.Application.Dtos.Sales.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuayaquilBank.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly ISalesAppService _appService;

        public SalesController(ISalesAppService appService)
        {
            _appService = appService;
        }

        /// <summary>
        /// Crea una nueva factura y descuenta el stock de los lotes correspondientes.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateInvoiceRequestDto request)
        {
            var result = await _appService.CreateInvoiceAsync(request);

            // Retornamos un 201 Created y apuntamos conceptualmente al endpoint de obtención por ID
            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
        }

        /// <summary>
        /// Obtiene el listado de facturas paginado y filtrado por cliente o secuencia.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponseDto<InvoiceResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedAsync([FromQuery] PaginationRequestDto request)
        {
            var result = await _appService.GetPagedInvoicesAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el detalle general de una factura por su identificador único.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _appService.GetInvoiceByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { Message = $"La factura con ID {id} no fue localizada en este entorno." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Genera y descarga el documento físico PDF profesional de la factura usando QuestPDF.
        /// </summary>
        [HttpGet("{id:guid}/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadPdfAsync(Guid id)
        {
            byte[] pdfBytes = await _appService.GenerateInvoicePdfAsync(id);

            string fileName = $"Factura_{id}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}