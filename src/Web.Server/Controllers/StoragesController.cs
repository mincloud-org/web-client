using Microsoft.AspNetCore.Mvc;
using MinCloud.Internal.SDK;
using MinCloud.Internal.SDK.HttpClients;
using Web.Server.Services;

namespace Web.Server.Controllers;

public class StoragesController(IMinCloudClient minCloudClient, IStorageService storageService) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var storage = await minCloudClient.Storage.GetByIdAsync(id, cancellationToken);
        return storage is null ? NotFound() : Ok(storage);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int offset = 0, int limit = 20, string? search = null, CancellationToken cancellationToken = default)
    {
        var storages = await minCloudClient.Storage.GetListAsync(offset, limit, search, cancellationToken);
        return Ok(storages);
    }
    
    [HttpGet("list-items")]
    public async Task<IActionResult> GetSelectListItems(string? search = null, CancellationToken cancellationToken = default)
    {
        var items = await storageService.GetStorageSelectListItemsAsync(search, cancellationToken);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStorageRequest request, CancellationToken cancellationToken)
    {
        var storage = await minCloudClient.Storage.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = storage!.Id }, storage);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStorageRequest request, CancellationToken cancellationToken)
    {
        var storage = await minCloudClient.Storage.UpdateAsync(id, request, cancellationToken);
        return storage is null ? NotFound() : Ok(storage);
    }
}
