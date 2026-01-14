using MinCloud.Internal.SDK;
using MinCloud.Internal.SDK.HttpClients;
using Web.Server.Extensions;
using Web.Server.Services;

namespace Web.Server.MultiTenant;

/// <summary>
/// Interface for resolving the current tenant
/// </summary>
public interface ITenantResolver
{
    string? TenantName { get; }
    Task<TenantDto?> ResolveAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Resolves tenant from the HTTP request host
/// </summary>
internal class HostBasedTenantResolver(IHttpContextAccessor httpContextAccessor, ITenantService tenantService) : ITenantResolver
{
    public string? TenantName
    {
        get
        {
            var host = httpContextAccessor.HttpContext?.Request.Host.Host;

            if (string.IsNullOrEmpty(host))
                return null;

            if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase) || System.Net.IPAddress.TryParse(host, out _))
            {
                return "dev";
            }

            // Example logic: extract subdomain as tenant name
            var segments = host.Split('.');
            if (segments.Length < 3)
                return null;
            return segments[0];
        }
    }

    public async Task<TenantDto?> ResolveAsync(CancellationToken cancellationToken = default)
    {
        var tenantName = TenantName;
        if (string.IsNullOrEmpty(tenantName))
            return null;

        return await tenantService.GetTenantByNameAsync(tenantName, cancellationToken);
    }
}
