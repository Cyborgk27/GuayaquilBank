using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Inventory.Request;
using GuayaquilBank.Application.Dtos.Inventory.Response;
using GuayaquilBank.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuayaquilBank.WebApi.Controllers
{
    /// <summary>
    /// Controlador encargado de la gestión del catálogo de productos, inventarios y reabastecimiento por lotes.
    /// Protegido bajo políticas de autenticación y aislamiento estricto por Tenant.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class ProductsController : BaseApiController
    {
        private readonly IProductAppService _appService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ProductsController"/>.
        /// </summary>
        /// <param name="appService">Servicio de aplicación encargado de la lógica del catálogo de inventario.</param>
        public ProductsController(IProductAppService appService)
        {
            _appService = appService;
        }

        /// <summary>
        /// Obtiene el catálogo de productos paginado, filtrado y ordenado perteneciente a la compañía actual.
        /// </summary>
        /// <param name="request">Criterios de paginación (Page, PageSize) y cadenas de filtrado por coincidencia.</param>
        /// <returns>Colección estructurada de productos disponibles en el stock corporativo.</returns>
        /// <response code="200">Retorna el listado paginado envuelto en la estructura global de éxito.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginationResponseDto<ProductResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationRequestDto request)
        {
            var result = await _appService.GetPagedProductsAsync(request);
            return ToResponse(result, "Catálogo de productos recuperado de forma exitosa.");
        }

        /// <summary>
        /// Obtiene el detalle de auditoría y stock completo de un producto específico mediante su ID único.
        /// </summary>
        /// <param name="id">Identificador único (GUID) del producto.</param>
        /// <returns>Ficha descriptiva detallada del producto seleccionado.</returns>
        /// <response code="200">Retorna la ficha del producto localizado.</response>
        /// <response code="404">Si el identificador no existe o pertenece a otro Tenant comercial.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _appService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse("El producto solicitado no existe en el catálogo de este Tenant.", null, StatusCodes.Status404NotFound));
            }

            return ToResponse(product, "Detalle del producto localizado con éxito.");
        }

        /// <summary>
        /// Crea un nuevo producto en el catálogo comercial e inicializa de manera atómica su primer lote de stock (opcional).
        /// </summary>
        /// <remarks>
        /// El comportamiento de negocio DDD validará de forma estricta que códigos SKU o códigos de barra no colisionen dentro del mismo Tenant.
        /// </remarks>
        /// <param name="request">DTO con la ficha técnica del producto, costos, precios y datos del lote inicial.</param>
        /// <returns>Los datos del producto persistido junto con sus metadatos de auditoría.</returns>
        /// <response code="201">El producto se insertó con éxito en el inventario.</response>
        /// <response code="400">Si el SKU está duplicado o los datos de precios/costos son inconsistentes.<///response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDto request)
        {
            var createdProduct = await _appService.CreateProductAsync(request);

            var response = ApiResponse<ProductResponseDto>.SuccessResponse(createdProduct, "Producto registrado y catalogado exitosamente.", StatusCodes.Status201Created);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, response);
        }

        /// <summary>
        /// Registra un nuevo lote de stock de reabastecimiento e incrementa las existencias generales para un producto existente.
        /// </summary>
        /// <param name="id">Identificador único (GUID) del producto al cual se le asocia la nueva mercancía.</param>
        /// <param name="request">Datos del lote comercial (Número de lote, cantidad física, fechas de expiración/fabricación y costos).</param>
        /// <returns>Sin contenido directo, confirma la inserción y recálculo de stock general de forma atómica.</returns>
        /// <response code="204">El lote se inyectó de forma satisfactoria recalculando el stock disponible del producto.</response>
        /// <response code="400">Si las cantidades ingresadas rompen reglas de valor numérico positivo.</response>
        /// <response code="404">Si el producto a reabastecer no fue localizado en el dominio organizacional.</response>
        [HttpPost("{id:guid}/batches")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddBatch(Guid id, [FromBody] CreateProductBatchRequestDto request)
        {
            await _appService.VoidAddBatchAsync(id, request);

            return ToResponse((object?)null, "Lote de inventario reabastecido y stock recalculado con éxito.", StatusCodes.Status204NoContent);
        }
    }
}