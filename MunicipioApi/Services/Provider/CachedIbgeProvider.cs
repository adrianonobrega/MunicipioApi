using MunicipioApi.Api.Services.Interfaces;
using MunicipioApi.Api.Models;
using MunicipioApi.Api.Extensions;

namespace MunicipioApi.Services.Provider
{
    public class CachedIbgeProvider : IIbgeProvider
    {
        private readonly IIbgeProvider _decoratedProvider;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachedIbgeProvider> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public CachedIbgeProvider(
            IIbgeProvider decoratedProvider,
            ICacheService cacheService,
            ILogger<CachedIbgeProvider> logger)
        {
            _decoratedProvider = decoratedProvider;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<PagedResponse<MunicipioResponse>> GetMunicipiosByUfAsync(string ufSigla, PaginationParams paginationParams)
        {
            string cacheKey = $"municipios_{ufSigla.ToUpper()}";
            _logger.LogInformation("Verificando cache para a chave: {CacheKey}", cacheKey);

            var municipiosEmCache = await _cacheService.GetAsync<IEnumerable<MunicipioResponse>>(cacheKey);

            IEnumerable<MunicipioResponse> municipiosCompletos;

            if (municipiosEmCache != null)
            {
                _logger.LogInformation("Cache HIT. Retornando dados do cache para a UF: {UF}", ufSigla);
                municipiosCompletos = municipiosEmCache;
            }
            else
            {
                _logger.LogInformation("Cache MISS. Buscando dados no provider real para a UF: {UF}", ufSigla);

                municipiosCompletos = await GetFullListFromProviderAsync(ufSigla);

                if (municipiosCompletos.Any())
                {
                    _logger.LogInformation("Armazenando {Count} municípios no cache para a UF: {UF}", municipiosCompletos.Count(), ufSigla);
                    await _cacheService.SetAsync(cacheKey, municipiosCompletos, _cacheDuration);
                }
            }

            int totalRecords = municipiosCompletos.Count();

            if (totalRecords > 0 && paginationParams.PageSize > 0)
            {
                int totalPages = (int)Math.Ceiling((double)totalRecords / paginationParams.PageSize);

                if (paginationParams.PageNumber > totalPages && totalPages > 0)
                {
                    _logger.LogWarning("PageNumber '{RequestedPage}' está fora do limite. Redirecionando para a última página válida: {LastPage}",
                        paginationParams.PageNumber, totalPages);

                    paginationParams.PageNumber = totalPages;
                }

                if (paginationParams.PageNumber <= 0)
                {
                    paginationParams.PageNumber = 1;
                }
            }

            return municipiosCompletos.ToPagedResponse(paginationParams);
        }

        private async Task<IEnumerable<MunicipioResponse>> GetFullListFromProviderAsync(string ufSigla)
        {
            var pagedResponseCompleto = await _decoratedProvider.GetMunicipiosByUfAsync(
                ufSigla,
                new PaginationParams { PageNumber = 1, PageSize = 10000 }
            );

            return pagedResponseCompleto?.Data ?? Enumerable.Empty<MunicipioResponse>();
        }
    }
}