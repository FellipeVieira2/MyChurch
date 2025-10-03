using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Common.Models;
using System.Linq.Expressions;

namespace MyChurch.Application.Common.Extensions
{
    /// <summary>
    /// Extension methods para facilitar paginação
    /// </summary>
    public static class PaginationExtensions
    {
        /// <summary>
        /// Converte um IQueryable em PaginatedList aplicando paginação
        /// </summary>
        public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await PaginatedList<T>.CreateAsync(source, pageNumber, pageSize, cancellationToken);
        }

        /// <summary>
        /// Converte um IQueryable em PaginatedList aplicando paginação com request
        /// </summary>
        public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> source,
            PaginatedRequest request,
            CancellationToken cancellationToken = default)
        {
            return await PaginatedList<T>.CreateAsync(
                source,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }

        /// <summary>
        /// Aplica ordenação dinâmica baseada em string
        /// </summary>
        public static IQueryable<T> ApplySort<T>(
            this IQueryable<T> source,
            string? sortBy,
            bool isDescending = false)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return source;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = typeof(T).GetProperty(sortBy);

            if (property == null)
                return source;

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var methodName = isDescending ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.PropertyType },
                source.Expression,
                Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<T>(resultExpression);
        }

        /// <summary>
        /// Aplica ordenação com PaginatedRequest
        /// </summary>
        public static IQueryable<T> ApplySort<T>(
            this IQueryable<T> source,
            PaginatedRequest request)
        {
            return source.ApplySort(request.SortBy, request.IsDescending);
        }

        /// <summary>
        /// Aplica paginação e ordenação em uma única chamada
        /// </summary>
        public static async Task<PaginatedList<T>> ToPaginatedListWithSortAsync<T>(
            this IQueryable<T> source,
            PaginatedRequest request,
            CancellationToken cancellationToken = default)
        {
            var sorted = source.ApplySort(request);
            return await sorted.ToPaginatedListAsync(request, cancellationToken);
        }

        /// <summary>
        /// Converte IEnumerable em PaginatedList (para listas em memória)
        /// </summary>
        public static PaginatedList<T> ToPaginatedList<T>(
            this IEnumerable<T> source,
            int pageNumber,
            int pageSize)
        {
            return PaginatedList<T>.Create(source, pageNumber, pageSize);
        }

        /// <summary>
        /// Converte IEnumerable em PaginatedList com request
        /// </summary>
        public static PaginatedList<T> ToPaginatedList<T>(
            this IEnumerable<T> source,
            PaginatedRequest request)
        {
            return PaginatedList<T>.Create(source, request.PageNumber, request.PageSize);
        }
    }
}
