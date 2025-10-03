using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.Common.Models
{
    /// <summary>
    /// Representa uma lista paginada genérica de itens
    /// </summary>
    /// <typeparam name="T">Tipo dos itens da lista</typeparam>
    public class PaginatedList<T>
    {
        /// <summary>
        /// Itens da página atual
        /// </summary>
        public IReadOnlyList<T> Items { get; }

        /// <summary>
        /// Número da página atual (baseado em 1)
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// Total de páginas disponíveis
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Total de itens em todas as páginas
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Quantidade de itens por página
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Indica se existe página anterior
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Indica se existe próxima página
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Índice do primeiro item da página atual (1-based)
        /// </summary>
        public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;

        /// <summary>
        /// Índice do último item da página atual (1-based)
        /// </summary>
        public int LastItemOnPage => Math.Min(FirstItemOnPage + PageSize - 1, TotalCount);

        public PaginatedList(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            PageSize = pageSize;
            Items = items;
        }

        /// <summary>
        /// Cria uma lista paginada a partir de um IQueryable
        /// </summary>
        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, 
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// Cria uma lista paginada a partir de uma lista em memória
        /// </summary>
        public static PaginatedList<T> Create(
            IEnumerable<T> source,
            int pageNumber,
            int pageSize)
        {
            var enumerable = source as IList<T> ?? source.ToList();
            var count = enumerable.Count;
            var items = enumerable
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// Cria uma lista paginada vazia
        /// </summary>
        public static PaginatedList<T> Empty(int pageNumber = 1, int pageSize = 10)
        {
            return new PaginatedList<T>(new List<T>(), 0, pageNumber, pageSize);
        }
    }
}
