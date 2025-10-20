using MunicipioApi.Api.Services.Interfaces;
using MunicipioApi.Api.Models;

namespace MunicipioApi.Api.Services
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

        public async Task<IEnumerable<MunicipioResponse>> GetMunicipiosByUfAsync(string ufSigla)
        {
            string cacheKey = $"municipios_{ufSigla.ToUpper()}";
            _logger.LogInformation("Verificando cache para a chave: {CacheKey}", cacheKey);

            var municipiosEmCache = await _cacheService.GetAsync<IEnumerable<MunicipioResponse>>(cacheKey);

            if (municipiosEmCache != null)
            {
                _logger.LogInformation("Cache HIT. Retornando dados do cache para a UF: {UF}", ufSigla);
                return municipiosEmCache;
            }

            _logger.LogInformation("Cache MISS. Buscando dados no provider real para a UF: {UF}", ufSigla);

            var municipiosDoProvider = await _decoratedProvider.GetMunicipiosByUfAsync(ufSigla);

            if (municipiosDoProvider != null && municipiosDoProvider.Any())
            {
                _logger.LogInformation("Armazenando {Count} municípios no cache para a UF: {UF}", municipiosDoProvider.Count(), ufSigla);
                await _cacheService.SetAsync(cacheKey, municipiosDoProvider, _cacheDuration);
            }

            return municipiosDoProvider;
        }
    }
}