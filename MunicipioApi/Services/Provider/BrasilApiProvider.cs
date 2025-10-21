using MunicipioApi.Api.Services.Interfaces;
using MunicipioApi.Api.Models;
using MunicipioApi.Api.Models.BrasilApi;
using MunicipioApi.Api.Extensions;
using System.Net;

namespace MunicipioApi.Services.Provider
{
    public class BrasilApiProvider : IIbgeProvider
    {
        private readonly HttpClient _httpClient;

        public BrasilApiProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResponse<MunicipioResponse>> GetMunicipiosByUfAsync(string ufSigla, PaginationParams paginationParams)
        {
            var response = await _httpClient.GetAsync($"/api/ibge/municipios/v1/{ufSigla}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Enumerable.Empty<MunicipioResponse>().ToPagedResponse(paginationParams);
            }

            await response.EnsureSuccessOrThrowProviderErrorAsync("Brasil API", ufSigla);

            var apiModels = await response.Content.ReadFromJsonAsync<IEnumerable<BrasilApiMunicipioModel>>();

            var municipios = apiModels?.Select(m => new MunicipioResponse
            {
                Name = m.nome,
                IbgeCode = m.codigo_ibge.ToString()
            }) ?? Enumerable.Empty<MunicipioResponse>();

            return municipios.ToPagedResponse(paginationParams);
        }
    }
}