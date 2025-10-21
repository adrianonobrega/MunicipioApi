using Microsoft.AspNetCore.Mvc;
using Moq;
using MunicipioApi.Api.Controllers;
using MunicipioApi.Api.Models;
using MunicipioApi.Api.Services.Interfaces;
using MunicipioApi.Api.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace MunicipioApi.Tests.UnitTests
{
    public class MunicipiosControllerTests
    {
        private readonly Mock<IIbgeProvider> _mockProvider;
        private readonly MunicipiosController _controller;

        // Parâmetros de paginação padrão para serem usados em todos os testes
        private readonly PaginationParams _defaultPaginationParams = new PaginationParams();

        public MunicipiosControllerTests()
        {
            _mockProvider = new Mock<IIbgeProvider>();
            // Note: O IIbgeProvider provavelmente ainda tem a assinatura antiga, 
            // mas o mock será configurado de acordo com o que o Controller chama.
            _controller = new MunicipiosController(_mockProvider.Object);
        }

        // Teste de validação de tamanho da UF
        [Theory]
        [InlineData("S")]
        [InlineData("SPA")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public async Task GetMunicipiosByUf_WhenUfIsInvalidLength_ShouldReturnBadRequest(string invalidUf)
        {
            // O argumento _defaultPaginationParams é OBRIGATÓRIO agora
            var result = await _controller.GetMunicipiosByUf(invalidUf, _defaultPaginationParams);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetMunicipiosByUf_WhenProviderReturnsEmpty_ShouldReturnNotFound()
        {
            const string ufSigla = "XY";

            // O mock deve simular o retorno de uma lista vazia
            _mockProvider.Setup(p => p.GetMunicipiosByUfAsync(ufSigla))
                .ReturnsAsync(Enumerable.Empty<MunicipioResponse>());

            // O argumento _defaultPaginationParams é OBRIGATÓRIO agora
            var result = await _controller.GetMunicipiosByUf(ufSigla, _defaultPaginationParams);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        // Teste de sucesso ajustado para o novo PagedResponse
        [Fact]
        public async Task GetMunicipiosByUf_WhenSuccessful_ShouldReturnPagedResponse()
        {
            const string ufSigla = "RS";
            var mockMunicipios = new List<MunicipioResponse>
            {
                new MunicipioResponse { Name = "Municipio 1", IbgeCode = "1" },
                new MunicipioResponse { Name = "Municipio 2", IbgeCode = "2" }
            };

            // 1. Configurar o mock para retornar a lista completa
            _mockProvider.Setup(p => p.GetMunicipiosByUfAsync(ufSigla))
                .ReturnsAsync(mockMunicipios);

            // 2. Chamar o Controller com os parâmetros padrão (PageNumber=1, PageSize=10)
            var result = await _controller.GetMunicipiosByUf(ufSigla, _defaultPaginationParams);

            // 3. Verificar o tipo de retorno (deve ser 200 OK)
            var okResult = Assert.IsType<OkObjectResult>(result);

            // 4. CORREÇÃO: Verifica que o valor retornado é o PagedResponse e não a lista crua.
            var pagedResponse = Assert.IsType<PagedResponse<MunicipioResponse>>(okResult.Value);

            // 5. Verifica os metadados da paginação
            Assert.Equal(1, pagedResponse.PageNumber);
            Assert.Equal(2, pagedResponse.TotalRecords);
            Assert.Equal(2, pagedResponse.Data.Count());
        }
    }
}
