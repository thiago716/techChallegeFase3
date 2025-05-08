using Application.External;
using Application.Service.Interface;
using Application.ViewModel;
using Application.Service.Base;
using Polly.Extensions.Http;
using Polly;

namespace WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

            services.AddHttpClient<ContactExternalService>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetTimeoutPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHttpClient<DirectDistanceDialingExternalService>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetTimeoutPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddMemoryCache();


            services.AddScoped<IContactService, ContactExternalService>();
            services.AddScoped<IDirectDistanceDialingService, DirectDistanceDialingExternalService>();

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"[Retry] Tentativa {retryAttempt} falhou. Retentando em {timespan.TotalSeconds}s");
                    });

        private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy() =>
            Policy.TimeoutAsync<HttpResponseMessage>(10);

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30),
                    onBreak: (outcome, breakDelay) =>
                    {
                        Console.WriteLine($"[CircuitBreaker] Circuito aberto por {breakDelay.TotalSeconds}s.");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("[CircuitBreaker] Circuito fechado. Requisições normalizadas.");
                    });
    }
}