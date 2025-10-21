using Microsoft.Extensions.Caching.Memory;
using MunicipioApi.Api.Services.Interfaces;

namespace MunicipioApi.Api.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<T?> GetAsync<T>(string key) where T : class
        {
            _memoryCache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow) where T : class
        {
            var options = new MemoryCacheEntryOptions();

            if (absoluteExpirationRelativeToNow.HasValue)
            {
                options.SetAbsoluteExpiration(absoluteExpirationRelativeToNow.Value);
            }
            else
            {
                options.SetAbsoluteExpiration(TimeSpan.FromHours(1));
            }

            _memoryCache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}