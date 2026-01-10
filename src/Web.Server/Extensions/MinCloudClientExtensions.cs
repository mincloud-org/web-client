using MinCloud.Internal.SDK.HttpClients;

namespace Web.Server.Extensions;

public static class MinCloudClientExtensions
{
    public static async Task<TenantDto?> GetByNameAsync(this ITenantApiClient tenantApiClient, string name, CancellationToken cancellationToken = default)
    {
        var tenants = await tenantApiClient.GetAllAsync(cancellationToken);
        return tenants?.FirstOrDefault(t => t.Name == name);
    }
}
