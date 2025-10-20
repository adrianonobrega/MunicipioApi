using Microsoft.AspNetCore.Mvc;
using Moq;
using MunicipioApi.Api.Controllers;
using MunicipioApi.Api.Models;
using MunicipioApi.Api.Services.Interfaces;

namespace MunicipioApi.Tests.UnitTests
{
    public class MunicipiosControllerTests
    {
        private readonly Mock<IIbgeProvider> _mockProvider;
        private readonly MunicipiosController _controller;

        public MunicipiosControllerTests()
        {
            _mockProvider = new Mock<IIbgeProvider>();
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
            var result = await _controller.GetMunicipiosByUf(invalidUf);


            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetMunicipiosByUf_WhenProviderReturnsEmpty_ShouldReturnNotFound()
        {
            const string ufSigla = "XY";

            _mockProvider.Setup(p => p.GetMunicipiosByUfAsync(ufSigla))
                .ReturnsAsync(new List<MunicipioResponse>());

            var result = await _controller.GetMunicipiosByUf(ufSigla);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}