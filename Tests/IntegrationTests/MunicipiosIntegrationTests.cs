using Microsoft.AspNetCore.Mvc.Testing;
using MunicipioApi.Api.Models;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace MunicipioApi.Tests.IntegrationTests
{

    public class MunicipiosIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public MunicipiosIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetMunicipiosByUf_ValidUf_ReturnsOkAndData()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/municipios/RS");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var municipios = JsonSerializer.Deserialize<List<MunicipioResponse>>(content);

            Assert.NotEmpty(municipios!);
        }

        [Fact]
        public async Task GetMunicipiosByUf_InvalidUfSigla_ReturnsNotFound()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/municipios/XX");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetMunicipiosByUf_UfWithInvalidLength_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/municipios/RSA");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}