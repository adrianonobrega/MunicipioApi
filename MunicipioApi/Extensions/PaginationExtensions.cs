using MunicipioApi.Api.Models;

namespace MunicipioApi.Api.Extensions
{
    public static class PaginationExtensions
    {
        public static PagedResponse<T> ToPagedResponse<T>(
            this IEnumerable<T> source,
            PaginationParams paginationParams) where T : class
        {
            var totalRecords = source.Count();

            var pageNumber = paginationParams.PageNumber < 1 ? 1 : paginationParams.PageNumber;

            var items = source
                .Skip((pageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToList();

            return new PagedResponse<T>(items, pageNumber,
                paginationParams.PageSize, totalRecords);
        }
    }
}