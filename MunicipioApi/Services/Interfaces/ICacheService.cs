namespace MunicipioApi.Api.Services.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow) where T : class;
        Task RemoveAsync(string key);
    }
}