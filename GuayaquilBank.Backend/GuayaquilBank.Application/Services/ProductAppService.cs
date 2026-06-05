using GuayaquilBank.Application.Contracts;
using GuayaquilBank.Application.Dtos.Common;
using GuayaquilBank.Application.Dtos.Inventory.Request;
using GuayaquilBank.Application.Dtos.Inventory.Response;
using GuayaquilBank.Domain.Entities.Inventory;
using GuayaquilBank.Domain.Exceptions;
using GuayaquilBank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuayaquilBank.Application.Services
{
    public class ProductAppService : IProductAppService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ProductAppService> _logger;
        private readonly ICurrentUser _currentUser;

        public ProductAppService(IApplicationDbContext context, ILogger<ProductAppService> logger, ICurrentUser currentUser)
        {
            _context = context;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateProductRequestDto request)
        {
            _logger.LogInformation("Creando producto '{Name}' a través de catálogos DDD unificados", request.Name);
            var companyId = _currentUser.CompanyId ?? Guid.Empty;

            var skuExists = await _context.Products
                .AnyAsync(p => p.CompanyId == companyId && p.Sku.ToUpper() == request.Sku.Trim().ToUpper());

            if (skuExists)
            {
                throw new UserFriendlyException($"El código SKU '{request.Sku}' ya se encuentra registrado en el catálogo.");
            }

            var product = new Product(
                id: Guid.NewGuid(),
                companyId: companyId,
                sku: request.Sku,
                name: request.Name,
                description: request.Description ?? string.Empty,
                minimumStockAlert: 0
            );

            if (request.InitialBatch != null)
            {
                var initialBatch = new ProductBatch(
                    id: Guid.NewGuid(),
                    productId: product.Id,
                    batchNumber: request.InitialBatch.BatchNumber,
                    quantity: request.InitialBatch.Quantity,
                    unitCost: request.InitialBatch.UnitCost,
                    manufacturedAt: request.InitialBatch.ManufacturedAt,
                    expirationDate: request.InitialBatch.ExpirationDate
                );

                _context.ProductBatches.Add(initialBatch);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync(default);

            return MapToProductResponseDto(product);
        }

        public async Task VoidAddBatchAsync(Guid productId, CreateProductBatchRequestDto request)
        {
            _logger.LogInformation("Insertando nuevo lote '{BatchNumber}' para el producto {ProductId}", request.BatchNumber, productId);

            var productExists = await _context.Products
                .AnyAsync(p => p.Id == productId && p.CompanyId == _currentUser.CompanyId);

            if (!productExists)
            {
                throw new UserFriendlyException("El producto solicitado no existe en su catálogo.");
            }

            var newBatch = new ProductBatch(
                id: Guid.NewGuid(),
                productId: productId,
                batchNumber: request.BatchNumber,
                quantity: request.Quantity,
                unitCost: request.UnitCost,
                manufacturedAt: request.ManufacturedAt,
                expirationDate: request.ExpirationDate
            );

            _context.ProductBatches.Add(newBatch);
            await _context.SaveChangesAsync(default);
        }

        public async Task<PaginationResponseDto<ProductResponseDto>> GetPagedProductsAsync(PaginationRequestDto request)
        {
            var query = _context.Products
                .Include(p => p.Batches)
                .Where(p => p.CompanyId == _currentUser.CompanyId && p.IsActive)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToUpper();
                query = query.Where(p => p.Name.ToUpper().Contains(search) || p.Sku.ToUpper().Contains(search));
            }

            var totalItems = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = products.Select(MapToProductResponseDto).ToList();

            return new PaginationResponseDto<ProductResponseDto>(dtos, totalItems, request.Page, request.PageSize);
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(Guid productId)
        {
            var product = await _context.Products
                .Include(p => p.Batches)
                .FirstOrDefaultAsync(p => p.Id == productId && p.CompanyId == _currentUser.CompanyId);

            if (product == null) return null;

            return MapToProductResponseDto(product);
        }

        private static ProductResponseDto MapToProductResponseDto(Product product)
        {
            var activeBatches = product.Batches?
                .Where(b => b.Quantity > 0)
                .Select(b => new BatchResponseDto
                {
                    Id = b.Id,
                    UnitCost = b.UnitCost,
                    CurrentQuantity = b.Quantity,
                    ExpirationDate = b.ExpirationDate
                }).ToList() ?? new List<BatchResponseDto>();

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Description = product.Description,
                TotalStock = activeBatches.Sum(b => b.CurrentQuantity),
                ActiveBatches = activeBatches
            };
        }
    }
}