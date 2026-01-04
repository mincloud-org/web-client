using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MinCloud.Internal.SDK;


public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddMinCloudInternalApiClient(
        this IServiceCollection services,
        string baseUrl,
        bool useApiGateway = false,
        TimeSpan? timeout = null,
        IDictionary<string, string>? defaultHeaders = null,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        return services.AddMinCloudInternalApiClient(options =>
        {
            options.BaseUrl = baseUrl;
            options.UseApiGateway = useApiGateway;
            options.Timeout = timeout ?? TimeSpan.FromSeconds(60);
            options.DefaultHeaders = defaultHeaders;
            options.JsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };
        });
    }

    /// <summary>
    /// Adds MinCloud API client services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Configuration action for Deepin API options</param>
    /// <returns>The service collection for method chaining</returns>
    public static IHttpClientBuilder AddMinCloudInternalApiClient(this IServiceCollection services, Action<MinCloudApiOptions> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // Configure options
        services.Configure(configure);
        services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
        services.AddScoped<ITokenCredential, HttpTokenCredential>();

        // Add HTTP client
        return services.AddHttpClient<IMinCloudClient, MinCloudClient>((serviceProvider, httpClient) =>
         {
             var options = serviceProvider.GetRequiredService<IOptions<MinCloudApiOptions>>().Value;

             if (!string.IsNullOrEmpty(options.BaseUrl))
             {
                 httpClient.BaseAddress = new Uri(options.BaseUrl);
             }

             httpClient.Timeout = options.Timeout;
             if (options.DefaultHeaders != null)
             {
                 foreach (var header in options.DefaultHeaders)
                 {
                     httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                 }
             }
         }).AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
    }
}
