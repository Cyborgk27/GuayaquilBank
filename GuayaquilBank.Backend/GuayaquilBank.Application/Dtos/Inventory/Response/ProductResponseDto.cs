namespace GuayaquilBank.Application.Dtos.Inventory.Response
{
    /// <summary>
    /// DTO que expone la información detallada de un producto junto con el cálculo consolidado de su inventario.
    /// </summary>
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? Description { get; set; }

        /// <summary>
        /// Sumatoria total de las cantidades disponibles en todos los lotes activos de este producto.
        /// </summary>
        public int TotalStock { get; set; }

        /// <summary>
        /// Listado de lotes que actualmente respaldan el stock físico de este producto.
        /// </summary>
        public List<BatchResponseDto> ActiveBatches { get; set; } = new();
    }
}