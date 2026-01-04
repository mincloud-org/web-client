using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinCloud.Internal.SDK.HttpClients;

namespace MinCloud.Internal.SDK;

public interface IMinCloudClient
{
    ISpaceApiClient Space { get; }
}

public class MinCloudClient(ILogger<MinCloudClient> logger, IOptions<MinCloudApiOptions> options, HttpClient httpClient) : IMinCloudClient
{
    private Lazy<ISpaceApiClient> _spaceApiClient = new(() => new SpaceApiClient(httpClient, options, logger));
    public ISpaceApiClient Space => _spaceApiClient.Value;
}
