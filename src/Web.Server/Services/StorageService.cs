using System;
using MinCloud.Internal.SDK;
using MinCloud.Internal.SDK.HttpClients;
using Web.Server.Models;

namespace Web.Server.Services;

public interface IStorageService
{
    Task<List<SelectListItem>> GetStorageSelectListItemsAsync(string? search, CancellationToken cancellationToken = default);
}

public class StorageService(IMinCloudClient minCloudClient) : IStorageService
{
    public async Task<List<SelectListItem>> GetStorageSelectListItemsAsync(string? search, CancellationToken cancellationToken = default)
    {
        var result = await minCloudClient.Storage.GetListAsync(0, 100, search, cancellationToken);
        if (result is null || result.Items.Count() == 0)
            return new List<SelectListItem>();
        return result.Items.Select(s => new SelectListItem(s.Id.ToString(), s.Name, s.Status != StorageStatus.Active)).ToList();
    }
}
