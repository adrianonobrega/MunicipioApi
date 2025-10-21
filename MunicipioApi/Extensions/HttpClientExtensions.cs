using MunicipioApi.Api.Exceptions;

namespace MunicipioApi.Api.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task EnsureSuccessOrThrowProviderErrorAsync(
            this HttpResponseMessage response,
            string providerName,
            string ufSigla)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            string errorContent = await response.Content.ReadAsStringAsync();

            throw new ProviderIndisponivelException(
                $"O provedor {providerName} retornou erro ({response.StatusCode}) para a UF: {ufSigla}. " +
                $"Detalhes da resposta: {errorContent.Trim()}");
        }
    }
}