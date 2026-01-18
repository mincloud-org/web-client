using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MinCloud.Internal.SDK.HttpClients;

public enum UserStorageType
{
    AwsS3,
    AzureBlob
}
public enum StorageType
{
    BuiltIn,
    AwsS3,
    AzureBlob
}

public enum StorageStatus
{
    Active,
    Inactive,
    Error
}

public record CreateStorageRequest(UserStorageType Type, string Name, JsonObject CredentialsJson, string? Description);

public record UpdateStorageRequest(string? Name, string? CredentialsJson);

public record StorageDto(Guid Id, Guid CreatedBy, string Name, StorageType Type, StorageStatus Status, string? Error, DateTimeOffset? DeletedAt, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, long QuotaBytes, long UsedBytes);

public interface IStorageApiClient
{
    Task<StorageDto?> CreateAsync(CreateStorageRequest request, CancellationToken cancellationToken = default);
    Task<StorageDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<StorageDto>?> GetListAsync(int offset = 0, int limit = 20, string? search = null, CancellationToken cancellationToken = default);
    Task<StorageDto?> UpdateAsync(Guid id, UpdateStorageRequest request, CancellationToken cancellationToken = default);
}

public class StorageApiClient(HttpClient httpClient, IOptions<MinCloudApiOptions> options, ILogger logger, ITokenCredential tokenCredential) :
HttpClientBase(httpClient, options, logger, tokenCredential), IStorageApiClient
{
    protected override string PathBase => "storage";
    protected override bool UseClientCredentialsToken => false;
    private const string ApiPrefix = "/api/v1/storages";

    public async Task<StorageDto?> CreateAsync(CreateStorageRequest request, CancellationToken cancellationToken = default)
    {
        return await PostAsync<StorageDto>($"{ApiPrefix}", request, cancellationToken);
    }

    public async Task<StorageDto?> UpdateAsync(Guid id, UpdateStorageRequest request, CancellationToken cancellationToken = default)
    {
        return await PutAsync<StorageDto>($"{ApiPrefix}/{id}", request, cancellationToken);
    }

    public async Task<StorageDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<StorageDto>($"{ApiPrefix}/{id}", cancellationToken);
    }

    public async Task<PagedResult<StorageDto>?> GetListAsync(int offset = 0, int limit = 20, string? search = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, object?>
        {
            ["offset"] = offset,
            ["limit"] = limit
        };
        if (!string.IsNullOrEmpty(search))
        {
            queryParams["search"] = search;
        }
        var queryString = BuildQueryString(queryParams);
        return await GetAsync<PagedResult<StorageDto>>($"{ApiPrefix}{queryString}", cancellationToken);
    }
}
