namespace GuayaquilBank.Application.Dtos.Common
{
    public class PaginationRequestDto
    {
        private int _page = 1;
        private int _pageSize = 10;

        /// <summary>
        /// Número de página actual (Basado en 1).
        /// </summary>
        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Cantidad de registros por página.
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : (value > 100 ? 100 : value);
        }

        /// <summary>
        /// Término de búsqueda global (Filtro por texto plano).
        /// </summary>
        public string? Search { get; set; }
    }
}