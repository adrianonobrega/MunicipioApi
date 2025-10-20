using Microsoft.AspNetCore.Mvc;
using MunicipioApi.Api.Models;
using MunicipioApi.Api.Services.Interfaces;
using MunicipioApi.Api.Exceptions;

namespace MunicipioApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MunicipiosController : ControllerBase
    {
        private readonly IIbgeProvider _ibgeProvider;

        public MunicipiosController(IIbgeProvider ibgeProvider)
        {
            _ibgeProvider = ibgeProvider;
        }

        [HttpGet("{uf}")]
        [ProducesResponseType(typeof(IEnumerable<MunicipioResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetMunicipiosByUf([FromRoute] string uf)
        {
            if (string.IsNullOrWhiteSpace(uf) || uf.Length != 2)
            {
                return BadRequest("A sigla da UF deve conter exatamente 2 caracteres.");
            }

            try
            {
                var municipios = await _ibgeProvider.GetMunicipiosByUfAsync(uf.ToUpper());

                if (municipios == null || !municipios.Any())
                {

                    return NotFound($"Nenhum município encontrado para a UF: {uf.ToUpper()}. Verifique se a sigla é válida.");
                }
                return Ok(municipios);
            }
            catch (ProviderIndisponivelException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    error = "Provedor de dados externo indisponível.",
                    details = ex.Message
                });
            }
        }
    }
}