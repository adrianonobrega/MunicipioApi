using MunicipioApi.Api.Services;
using MunicipioApi.Api.Services.Interfaces;

namespace MunicipioApi.Api.Extensions
{
    public static class ProviderExtensions
    {
        public static IServiceCollection AddIbgeProvider(this IServiceCollection services, IConfiguration configuration)
        {
            var activeProvider = configuration["IbgeApiConfig:ActiveProvider"];

            var brasilApiUrl = configuration["IbgeApiConfig:BrasilApiUrl"]
                               ?? throw new InvalidOperationException("BrasilApiUrl não configurada em IbgeApiConfig.");
            var ibgeApiUrl = configuration["IbgeApiConfig:IbgeApiUrl"]
                             ?? throw new InvalidOperationException("IbgeApiUrl não configurada em IbgeApiConfig.");

            services.AddHttpClient();

            if (activeProvider?.ToUpper() == "BRASIL_API")
            {
                services.AddHttpClient<IIbgeProvider, BrasilApiProvider>(client =>
                {
                    client.BaseAddress = new Uri(brasilApiUrl);
                });
            }
            else if (activeProvider?.ToUpper() == "IBGE_API")
            {
                services.AddHttpClient<IIbgeProvider, IbgeApiProvider>(client =>
                {
                    client.BaseAddress = new Uri(ibgeApiUrl);
                });
            }
            else
            {
                throw new InvalidOperationException($"Provedor IBGE '{activeProvider}' não é válido. Use BRASIL_API ou IBGE_API.");
            }

            return services;
        }
    }
}