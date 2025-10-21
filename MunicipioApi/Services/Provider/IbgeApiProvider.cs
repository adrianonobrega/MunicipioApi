using MunicipioApi.Api.Services.Interfaces;
using MunicipioApi.Api.Models;
using MunicipioApi.Api.Models.IbgeApi;
using MunicipioApi.Api.Extensions;

namespace MunicipioApi.Services.Provider
{
    public class IbgeApiProvider : IIbgeProvider
    {
        private readonly HttpClient _httpClient;

        public IbgeApiProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResponse<MunicipioResponse>> GetMunicipiosByUfAsync(string ufSigla, PaginationParams paginationParams)
        {
            var response = await _httpClient.GetAsync($"/api/v1/localidades/estados/{ufSigla}/municipios");

            await response.EnsureSuccessOrThrowProviderErrorAsync("IBGE API", ufSigla);

            var apiModels = await response.Content.ReadFromJsonAsync<IEnumerable<IbgeApiMunicipioModel>>();

            var municipios = apiModels?.Select(m => new MunicipioResponse
            {
                Name = m.nome,
                IbgeCode = m.id.ToString()
            }) ?? Enumerable.Empty<MunicipioResponse>();

            return municipios.ToPagedResponse(paginationParams);
        }
    }
}