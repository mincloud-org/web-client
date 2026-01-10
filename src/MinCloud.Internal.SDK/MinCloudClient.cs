using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinCloud.Internal.SDK.HttpClients;

namespace MinCloud.Internal.SDK;

public interface IMinCloudClient
{
    ISpaceApiClient Space { get; }
    ITenantApiClient Tenant { get; }
}

public class MinCloudClient(ILogger<MinCloudClient> logger, IOptions<MinCloudApiOptions> options, HttpClient httpClient, ITokenCredential tokenCredential) : IMinCloudClient
{
    private Lazy<ISpaceApiClient> _spaceApiClient = new(() => new SpaceApiClient(httpClient, options, logger, tokenCredential));
    private Lazy<ITenantApiClient> _tenantApiClient = new(() => new TenantApiClient(httpClient, options, logger, tokenCredential));

    public ISpaceApiClient Space => _spaceApiClient.Value;

    public ITenantApiClient Tenant => _tenantApiClient.Value;
}
