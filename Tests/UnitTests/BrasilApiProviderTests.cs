using Xunit;
using Moq;
using Moq.Protected;
using MunicipioApi.Api.Services;
using MunicipioApi.Api.Exceptions;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace MunicipioApi.Tests.UnitTests
{
    public class BrasilApiProviderTests
    {
        // Método de teste 1: Deve retornar uma lista vazia quando a API externa retornar 404 (UF inválida)
        [Fact]
        public async Task GetMunicipiosByUfAsync_WhenApiReturnsNotFound_ShouldReturnEmptyList()
        {
            const string ufSigla = "SW";

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound, // Simula o 404
                    Content = new StringContent("", Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://dummy.brasilapi.com.br")
            };

            var provider = new BrasilApiProvider(httpClient);

            var result = await provider.GetMunicipiosByUfAsync(ufSigla);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
        
        // Método de teste 2: Deve lançar ProviderIndisponivelException quando a API retornar 500 (Falha no Provedor)
        [Fact]
        public async Task GetMunicipiosByUfAsync_WhenApiReturnsServerError_ShouldThrowException()
        {
            const string ufSigla = "RS"; 
            
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://dummy.brasilapi.com.br")
            };
            var provider = new BrasilApiProvider(httpClient);

            await Assert.ThrowsAsync<ProviderIndisponivelException>(() => 
                provider.GetMunicipiosByUfAsync(ufSigla));
        }

        // Adicionar teste de sucesso
        [Fact]
        public async Task GetMunicipiosByUfAsync_WhenApiReturnsSuccess_ShouldMapCorrectly()
        {
            const string ufSigla = "SP";
            var successJson = @"[
                { ""codigo_ibge"": 3550308, ""nome"": ""São Paulo"" },
                { ""codigo_ibge"": 3509902, ""nome"": ""Campinas"" }
            ]";
            
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(successJson, Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://dummy.brasilapi.com.br")
            };
            var provider = new BrasilApiProvider(httpClient);

            var result = await provider.GetMunicipiosByUfAsync(ufSigla);

            Assert.Equal(2, result.Count());
            Assert.Contains(result, m => m.Name == "São Paulo" && m.IbgeCode == "3550308");
            Assert.Contains(result, m => m.Name == "Campinas" && m.IbgeCode == "3509902");
        }
    }
}