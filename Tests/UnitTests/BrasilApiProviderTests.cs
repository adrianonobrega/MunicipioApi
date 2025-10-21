using Xunit;
using Moq;
using Moq.Protected;
using MunicipioApi.Api.Exceptions;
using MunicipioApi.Api.Models;
using MunicipioApi.Services.Provider;
using System.Net;
using System.Text;

namespace MunicipioApi.Tests.UnitTests
{
    public class BrasilApiProviderTests
    {
        private readonly PaginationParams _defaultPaginationParams = new PaginationParams();

        private const string TestBaseUrl = "https://brasilapi.com.br";

        private BrasilApiProvider CreateProvider(HttpMessageHandler mockHandler)
        {
            var httpClient = new HttpClient(mockHandler)
            {
                BaseAddress = new Uri(TestBaseUrl)
            };
            return new BrasilApiProvider(httpClient);
        }

        [Fact]
        public async Task GetMunicipiosByUfAsync_WhenApiReturnsNotFound_ShouldReturnEmptyPagedResponse()
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
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("", Encoding.UTF8, "application/json")
                });

            var provider = CreateProvider(mockHttpMessageHandler.Object);

            var result = await provider.GetMunicipiosByUfAsync(ufSigla, _defaultPaginationParams);

            Assert.NotNull(result);
            Assert.Empty(result.Data);
            Assert.Equal(0, result.TotalRecords);
        }

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

            var provider = CreateProvider(mockHttpMessageHandler.Object);

            await Assert.ThrowsAsync<ProviderIndisponivelException>(() =>
                provider.GetMunicipiosByUfAsync(ufSigla, _defaultPaginationParams));
        }

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

            var provider = CreateProvider(mockHttpMessageHandler.Object);

            var result = await provider.GetMunicipiosByUfAsync(ufSigla, _defaultPaginationParams);

            Assert.Equal(2, result.Data.Count());
            Assert.Contains(result.Data, m => m.Name == "São Paulo" && m.IbgeCode == "3550308");
        }
    }
}