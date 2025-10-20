using MunicipioApi.Api.Services.Interfaces;
using MunicipioApi.Api.Models;
using MunicipioApi.Api.Models.IbgeApi;
using MunicipioApi.Api.Exceptions;


namespace MunicipioApi.Api.Services
{
    public class IbgeApiProvider : IIbgeProvider
    {
        private readonly HttpClient _httpClient;

        public IbgeApiProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<MunicipioResponse>> GetMunicipiosByUfAsync(string ufSigla)
        {
            var response = await _httpClient.GetAsync($"/api/v1/localidades/estados/{ufSigla}/municipios");

            if (!response.IsSuccessStatusCode)
            {
                throw new ProviderIndisponivelException($"O provedor IBGE API retornou erro ({response.StatusCode}) para a UF: {ufSigla}.");
            }

            var apiModels = await response.Content.ReadFromJsonAsync<IEnumerable<IbgeApiMunicipioModel>>();

            if (apiModels == null || !apiModels.Any())
            {
                return Enumerable.Empty<MunicipioResponse>();
            }

            return apiModels.Select(m => new MunicipioResponse
            {
                Name = m.nome,
                IbgeCode = m.id.ToString()
            });
        }
    }
}