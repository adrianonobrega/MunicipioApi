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

            services.AddHttpClient<BrasilApiProvider>(client =>
            {
                client.BaseAddress = new Uri(brasilApiUrl);
            });

            services.AddHttpClient<IbgeApiProvider>(client =>
            {
                client.BaseAddress = new Uri(ibgeApiUrl);
            });

            services.AddScoped<IIbgeProvider>(serviceProvider =>
            {
                var cacheService = serviceProvider.GetRequiredService<ICacheService>();
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                IIbgeProvider concreteProvider;

                if (activeProvider?.ToUpper() == "BRASIL_API")
                {
                    concreteProvider = serviceProvider.GetRequiredService<BrasilApiProvider>();
                }
                else if (activeProvider?.ToUpper() == "IBGE_API")
                {
                    concreteProvider = serviceProvider.GetRequiredService<IbgeApiProvider>();
                }
                else
                {
                    throw new InvalidOperationException($"Provedor IBGE '{activeProvider}' não é válido. Use BRASIL_API ou IBGE_API.");
                }

                var logger = loggerFactory.CreateLogger<CachedIbgeProvider>();
                return new CachedIbgeProvider(concreteProvider, cacheService, logger);
            });

            return services;
        }
    }
}