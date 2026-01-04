using System;
using System.Text.Json;

namespace MinCloud.Internal.SDK;

public class MinCloudApiOptions
{
    /// <summary>
    /// The base URL of the Deepin API
    /// </summary>
    public required string BaseUrl { get; set; }

    /// <summary>
    /// The timeout for HTTP requests
    /// </summary>
    public TimeSpan Timeout { get; set; }

    /// <summary>
    /// Additional headers to include with requests
    /// </summary>
    public IDictionary<string, string>? DefaultHeaders { get; set; }

    /// <summary>
    /// JSON serializer options for request and response serialization
    /// </summary>
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    /// <summary>
    /// Whether to use API Gateway
    /// </summary>
    public bool UseApiGateway { get; set; }
}
