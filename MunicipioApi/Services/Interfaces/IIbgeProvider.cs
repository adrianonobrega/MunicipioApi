using MunicipioApi.Api.Models;

namespace MunicipioApi.Api.Services.Interfaces
{
    public interface IIbgeProvider
    {
        Task<IEnumerable<MunicipioResponse>> GetMunicipiosByUfAsync(string ufSigla);
    }
}