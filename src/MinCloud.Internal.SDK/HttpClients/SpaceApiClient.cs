using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MinCloud.Internal.SDK.HttpClients;

public record SpaceDto(Guid Id, string Name, string? Description, Guid CreatedBy, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, Guid StorageId, string StorageContainer);
public record CreateSpaceRequest(string Name, string? Description, Guid StorageId);
public record UpdateSpaceRequest(string Name, string? Description);

public interface ISpaceApiClient
{
    Task<SpaceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<SpaceDto>?> GetListAsync(int offset = 0, int limit = 20, string? search = null, CancellationToken cancellationToken = default);
    Task<SpaceDto?> CreateAsync(CreateSpaceRequest dto, CancellationToken cancellationToken = default);
    Task<SpaceDto?> UpdateAsync(Guid id, UpdateSpaceRequest dto, CancellationToken cancellationToken = default);
}

internal class SpaceApiClient(HttpClient httpClient, IOptions<MinCloudApiOptions> options, ILogger logger, ITokenCredential tokenCredential) :
HttpClientBase(httpClient, options, logger, tokenCredential), ISpaceApiClient
{
    protected override string PathBase => "space";
    protected override bool UseClientCredentialsToken => false;
    private const string ApiPrefix = "/api/v1/spaces";

    public async Task<PagedResult<SpaceDto>?> GetListAsync(int offset = 0, int limit = 20, string? search = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, object?>
        {
            { "offset", offset},
            { "limit", limit }
        };
        if (!string.IsNullOrEmpty(search))
        {
            queryParams.Add("search", search);
        }
        var queryString = BuildQueryString(queryParams);

        return await GetAsync<PagedResult<SpaceDto>>($"{ApiPrefix}{queryString}", cancellationToken);
    }

    public async Task<SpaceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<SpaceDto?>($"{ApiPrefix}/{id}", cancellationToken);
    }

    public async Task<SpaceDto?> CreateAsync(CreateSpaceRequest dto, CancellationToken cancellationToken = default)
    {
        return await PostAsync<SpaceDto>(ApiPrefix, dto, cancellationToken);
    }

    public async Task<SpaceDto?> UpdateAsync(Guid id, UpdateSpaceRequest dto, CancellationToken cancellationToken = default)
    {
        return await PutAsync<SpaceDto>($"{ApiPrefix}/{id}", dto, cancellationToken);
    }
}
