using FluentValidation;

namespace MyChurch.Application.Common.Models
{
    /// <summary>
    /// Request base para queries paginadas
    /// </summary>
    public abstract class PaginatedRequest
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 10;

        /// <summary>
        /// Número da página (baseado em 1, padrão: 1)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Quantidade de itens por página (padrão: 10, máximo: 100)
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        /// <summary>
        /// Campo para ordenação (opcional)
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Direção da ordenação (asc ou desc, padrão: asc)
        /// </summary>
        public string SortDirection { get; set; } = "asc";

        /// <summary>
        /// Indica se a ordenação é descendente
        /// </summary>
        public bool IsDescending => SortDirection?.ToLower() == "desc";
    }

    /// <summary>
    /// Validador base para requests paginados
    /// </summary>
    public class PaginatedRequestValidator<T> : AbstractValidator<T> where T : PaginatedRequest
    {
        public PaginatedRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Número da página deve ser maior que zero");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Tamanho da página deve estar entre 1 e 100");

            RuleFor(x => x.SortDirection)
                .Must(x => string.IsNullOrEmpty(x) || x.ToLower() == "asc" || x.ToLower() == "desc")
                .WithMessage("Direção de ordenação deve ser 'asc' ou 'desc'");
        }
    }
}
