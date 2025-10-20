using MunicipioApi.Api.Services.Interfaces;
using MunicipioApi.Api.Models;            
using System.Net.Http;                  

namespace MunicipioApi.Api.Services
{
    public class IbgeApiProvider : IIbgeProvider
    {
        private readonly HttpClient _httpClient;

        public IbgeApiProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public Task<IEnumerable<MunicipioResponse>> GetMunicipiosByUfAsync(string ufSigla)
        {
            return Task.FromResult(Enumerable.Empty<MunicipioResponse>());
        }
    }
}