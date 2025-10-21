using Microsoft.AspNetCore.Mvc;
using Moq;
using MunicipioApi.Api.Controllers;
using MunicipioApi.Api.Extensions;
using MunicipioApi.Api.Models;
using MunicipioApi.Api.Services.Interfaces;


namespace MunicipioApi.Tests.UnitTests
{
    public class MunicipiosControllerTests
    {
        private readonly Mock<IIbgeProvider> _mockProvider;
        private readonly MunicipiosController _controller;

        private readonly PaginationParams _defaultPaginationParams = new PaginationParams();

        public MunicipiosControllerTests()
        {
            _mockProvider = new Mock<IIbgeProvider>();
            _controller = new MunicipiosController(_mockProvider.Object);
        }

        [Theory]
        [InlineData("S")]
        [InlineData("SPA")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public async Task GetMunicipiosByUf_WhenUfIsInvalidLength_ShouldReturnBadRequest(string invalidUf)
        {
            var result = await _controller.GetMunicipiosByUf(invalidUf, _defaultPaginationParams);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetMunicipiosByUf_WhenProviderReturnsEmpty_ShouldReturnNotFound()
        {
            const string ufSigla = "XY";

            _mockProvider.Setup(p => p.GetMunicipiosByUfAsync(ufSigla, It.IsAny<PaginationParams>()))
                .ReturnsAsync(Enumerable.Empty<MunicipioResponse>().ToPagedResponse(new PaginationParams()));

            var result = await _controller.GetMunicipiosByUf(ufSigla, _defaultPaginationParams);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetMunicipiosByUf_WhenSuccessful_ShouldReturnPagedResponse()
        {
            const string ufSigla = "RS";
            var mockMunicipios = new List<MunicipioResponse>
            {
                new MunicipioResponse { Name = "Municipio 1", IbgeCode = "1" },
                new MunicipioResponse { Name = "Municipio 2", IbgeCode = "2" }
            };

            var pagedResponse = mockMunicipios.ToPagedResponse(new PaginationParams { PageNumber = 1, PageSize = 10 });

            _mockProvider.Setup(p => p.GetMunicipiosByUfAsync(ufSigla, It.IsAny<PaginationParams>()))
                .ReturnsAsync(pagedResponse);

            var result = await _controller.GetMunicipiosByUf(ufSigla, _defaultPaginationParams);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnedPagedResponse = Assert.IsType<PagedResponse<MunicipioResponse>>(okResult.Value);

            Assert.Equal(1, returnedPagedResponse.PageNumber);
            Assert.Equal(2, returnedPagedResponse.TotalRecords);
            Assert.Equal(2, returnedPagedResponse.Data.Count());
        }
    }
}