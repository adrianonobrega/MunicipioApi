using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MunicipioApi.Api.Models;
using Xunit;

namespace MunicipioApi.Tests.IntegrationTests
{
    public class MunicipiosIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public MunicipiosIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("RS")]
        [InlineData("SC")]
        public async Task GetMunicipiosByUf_ValidUf_ReturnsOkAndData(string ufSigla)
        {
            var response = await _client.GetAsync($"/api/municipios/{ufSigla}");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<MunicipioResponse>>();

            Assert.NotNull(pagedResponse);
            Assert.NotNull(pagedResponse!.Data);

            Assert.True(pagedResponse.Data.Any());

            Assert.True(pagedResponse.TotalRecords > 0);
            Assert.Equal(1, pagedResponse.PageNumber);

            var primeiroMunicipio = pagedResponse.Data.First();
            Assert.False(string.IsNullOrWhiteSpace(primeiroMunicipio.Name));
            Assert.False(string.IsNullOrWhiteSpace(primeiroMunicipio.IbgeCode));
        }

        [Theory]
        [InlineData("S")]
        [InlineData("SPA")]
        public async Task GetMunicipiosByUf_InvalidUfLength_ReturnsBadRequest(string invalidUf)
        {
            var response = await _client.GetAsync($"/api/municipios/{invalidUf}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetMunicipiosByUf_InvalidUf_ReturnsNotFound()
        {
            var response = await _client.GetAsync($"/api/municipios/XX");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
