using System;
using System.Threading.Tasks;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Inventory.Request;
using GuayaquilBank.Application.Dtos.Inventory.Response;

namespace GuayaquilBank.Application.Contracts
{
    /// <summary>
    /// Define las reglas de negocio y flujos transaccionales para el catálogo de productos y control de lotes.
    /// </summary>
    public interface IProductAppService
    {
        /// <summary>
        /// Crea un nuevo producto en el catálogo de la compañía del usuario actual y registra su lote inicial si se provee.
        /// </summary>
        Task<ProductResponseDto> CreateProductAsync(CreateProductRequestDto request);

        /// <summary>
        /// Registra un nuevo lote de abastecimiento (adquisición de stock) para un producto existente.
        /// </summary>
        Task VoidAddBatchAsync(Guid productId, CreateProductBatchRequestDto request);

        /// <summary>
        /// Obtiene el catálogo de productos de la compañía actual filtrado y paginado con metadata optimizada para Angular.
        /// </summary>
        Task<PaginationResponseDto<ProductResponseDto>> GetPagedProductsAsync(PaginationRequestDto request);

        /// <summary>
        /// Obtiene el detalle completo de un producto incluyendo el desglose de todos sus lotes activos con stock disponible.
        /// </summary>
        Task<ProductResponseDto?> GetProductByIdAsync(Guid productId);
    }
}