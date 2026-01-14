using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MinCloud.Internal.SDK.HttpClients;

public abstract class HttpClientBase
{
    protected readonly HttpClient HttpClient;
    protected readonly MinCloudApiOptions Options;
    protected readonly ILogger Logger;
    protected readonly JsonSerializerOptions JsonOptions;
    protected readonly ITokenCredential TokenCredential;
    protected abstract string PathBase { get; }
    protected abstract bool UseClientCredentialsToken { get; }

    protected HttpClientBase(HttpClient httpClient, IOptions<MinCloudApiOptions> options, ILogger logger, ITokenCredential tokenCredential)
    {
        HttpClient = httpClient;
        Options = options.Value;
        Logger = logger;
        TokenCredential = tokenCredential;

        JsonOptions = options.Value.JsonSerializerOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new System.Text.Json.Serialization.JsonStringEnumConverter()
            }
        };
    }

    protected virtual async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        var response = await SendAsync(request, cancellationToken);

        return await HandleResponse<T>(response);
    }

    protected async Task<HttpResponseMessage> PostAsync(string endpoint, object? content = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(content, options: JsonOptions)
        };

        return await SendAsync(request, cancellationToken);
    }

    protected async Task<T?> PostAsync<T>(string endpoint, object? content = null, CancellationToken cancellationToken = default)
    {
        var response = await this.PostAsync(endpoint, content, cancellationToken);
        return await HandleResponse<T>(response);
    }

    protected async Task<HttpResponseMessage> PutAsync(string endpoint, object? content = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
        {
            Content = JsonContent.Create(content, options: JsonOptions)
        };

        return await SendAsync(request, cancellationToken);
    }

    protected async Task<T?> PutAsync<T>(string endpoint, object? content = null, CancellationToken cancellationToken = default)
    {
        var response = await PutAsync(endpoint, content, cancellationToken);
        return await HandleResponse<T>(response);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
        return await SendAsync(request, cancellationToken);
    }

    protected async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (Options.UseApiGateway)
        {
            request.RequestUri = new Uri($"{PathBase}/{request.RequestUri}");
        }
        if (UseClientCredentialsToken && request.Headers.Authorization == null)
        {
            var token = await TokenCredential.GetTokenAsync(cancellationToken);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        Logger.LogDebug("Sending {Method} request to {Endpoint}", request.Method, request.RequestUri);
        var response = await HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return response;
    }

    /// <summary>
    /// Handles the HTTP response and deserializes the content
    /// </summary>
    protected async Task<T?> HandleResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Logger.LogWarning("Request failed with status {StatusCode}: {Content}",
                response.StatusCode, content);

            var errorMessage = $"Request failed with status {response.StatusCode}";
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(content, JsonOptions);
                    if (errorResponse?.ContainsKey("message") == true)
                    {
                        errorMessage = errorResponse["message"].ToString() ?? errorMessage;
                    }
                }
                catch
                {
                    // Ignore JSON parsing errors
                }
            }
            throw new HttpRequestException(errorMessage, null, response.StatusCode);
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)content;
        }

        if (string.IsNullOrEmpty(content))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    /// <summary>
    /// Builds query string from parameters
    /// </summary>
    protected string BuildQueryString(Dictionary<string, object?> parameters)
    {
        var queryParams = parameters
            .Where(p => p.Value != null)
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value!.ToString() ?? string.Empty)}")
            .ToArray();

        return queryParams.Length > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }
}
