using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MinCloud.Internal.SDK.HttpClients;

public record TenantDto(Guid Id, string Name, string Description, Guid CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

public interface ITenantApiClient
{
    Task<IEnumerable<TenantDto>?> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public class TenantApiClient(HttpClient httpClient, IOptions<MinCloudApiOptions> options, ILogger logger, ITokenCredential tokenCredential) :
HttpClientBase(httpClient, options, logger, tokenCredential), ITenantApiClient
{
    protected override string PathBase => "tenant";

    protected override bool UseClientCredentialsToken => true;

    private const string ApiPrefix = "/api/v1/tenants";

    public async Task<IEnumerable<TenantDto>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<IEnumerable<TenantDto>?>(ApiPrefix, cancellationToken);
    }

    public async Task<TenantDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TenantDto?>($"{ApiPrefix}/{id}", cancellationToken);
    }
}
