using MunicipioApi.Api.Models;

namespace MunicipioApi.Api.Services.Interfaces
{
    public interface IIbgeProvider
    {
        Task<PagedResponse<MunicipioResponse>> GetMunicipiosByUfAsync(string ufSigla, PaginationParams paginationParams);
    }
}