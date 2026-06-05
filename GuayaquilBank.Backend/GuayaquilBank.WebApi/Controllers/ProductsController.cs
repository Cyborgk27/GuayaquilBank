using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Inventory.Request;
using GuayaquilBank.Application.Dtos.Inventory.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuayaquilBank.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductAppService _appService;

        public ProductsController(IProductAppService appService)
        {
            _appService = appService;
        }

        /// <summary>
        /// Obtiene el catálogo de productos paginado y filtrado de la compañía.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponseDto<ProductResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationRequestDto request)
        {
            var result = await _appService.GetPagedProductsAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el detalle completo de un producto por su ID único.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _appService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "El producto solicitado no existe en el catálogo." });
            }
            return Ok(product);
        }

        /// <summary>
        /// Crea un nuevo producto en el catálogo y registra su primer lote opcional.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDto request)
        {
            var createdProduct = await _appService.CreateProductAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        /// <summary>
        /// Registra un nuevo lote de stock de reabastecimiento para un producto existente.
        /// </summary>
        [HttpPost("{id:guid}/batches")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddBatch(Guid id, [FromBody] CreateProductBatchRequestDto request)
        {
            await _appService.VoidAddBatchAsync(id, request);
            return NoContent();
        }
    }
}