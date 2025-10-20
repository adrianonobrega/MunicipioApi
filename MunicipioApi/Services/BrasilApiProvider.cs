using MunicipioApi.Api.Services.Interfaces;
using MunicipioApi.Api.Models;
using MunicipioApi.Api.Models.BrasilApi;
using MunicipioApi.Api.Exceptions;       
using System.Net.Http.Json;             

namespace MunicipioApi.Api.Services
{
    public class BrasilApiProvider : IIbgeProvider
    {
        private readonly HttpClient _httpClient;

        public BrasilApiProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<MunicipioResponse>> GetMunicipiosByUfAsync(string ufSigla)
        {
            var response = await _httpClient.GetAsync($"/api/ibge/municipios/v1/{ufSigla}");

            if (!response.IsSuccessStatusCode)
            {
                throw new ProviderIndisponivelException($"O provedor Brasil API retornou erro ({response.StatusCode}) para a UF: {ufSigla}.");
            }

            var apiModels = await response.Content.ReadFromJsonAsync<IEnumerable<BrasilApiMunicipioModel>>();

            if (apiModels == null || !apiModels.Any())
            {
                return Enumerable.Empty<MunicipioResponse>();
            }

            return apiModels.Select(m => new MunicipioResponse
            {
                Name = m.nome,
                IbgeCode = m.codigo_ibge.ToString()
            });
        }
    }
}