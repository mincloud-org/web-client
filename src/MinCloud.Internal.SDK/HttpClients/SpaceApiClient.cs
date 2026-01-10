using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MinCloud.Internal.SDK.HttpClients;

public record SpaceDto(Guid Id, string Name, string? Description, Guid CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

public interface ISpaceApiClient
{
    Task<SpaceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<SpaceDto>?> GetAllAsync(CancellationToken cancellationToken = default);
}

internal class SpaceApiClient(HttpClient httpClient, IOptions<MinCloudApiOptions> options, ILogger logger, ITokenCredential tokenCredential) :
HttpClientBase(httpClient, options, logger, tokenCredential), ISpaceApiClient
{
    private const string ApiPrefix = "/api/v1/spaces";
    protected override string PathBase => "space";
    protected override bool UseClientCredentialsToken => false;

    public async Task<PagedResult<SpaceDto>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<PagedResult<SpaceDto>?>(ApiPrefix, cancellationToken);
    }

    public async Task<SpaceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<SpaceDto?>($"{ApiPrefix}/{id}", cancellationToken);
    }
}
