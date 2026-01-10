using System;
using Microsoft.Extensions.Caching.Memory;
using MinCloud.Internal.SDK;
using MinCloud.Internal.SDK.HttpClients;

namespace Web.Server.Services;

public interface ITenantService
{
    Task<IEnumerable<TenantDto>> GetAllTenantsAsync(CancellationToken cancellationToken);
    Task<TenantDto?> GetTenantByNameAsync(string name, CancellationToken cancellationToken);
}

public class TenantService(IMemoryCache memoryCache, IMinCloudClient minCloudClient) : ITenantService
{

    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync(CancellationToken cancellationToken)
    {
        const string cacheKey = "all_tenants";
        if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<TenantDto>? tenants))
        {
            tenants = await minCloudClient.Tenant.GetAllAsync(cancellationToken);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            memoryCache.Set(cacheKey, tenants, cacheEntryOptions);
        }
        return tenants ?? Array.Empty<TenantDto>();
    }

    public async Task<TenantDto?> GetTenantByNameAsync(string name, CancellationToken cancellationToken)
    {
        var tenants = await GetAllTenantsAsync(cancellationToken);
        return tenants.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
