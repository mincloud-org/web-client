using System.Security.Claims;

namespace Web.Server.MultiTenant;

/// <summary>
/// Extension methods for accessing tenant information
/// </summary>
public static class TenantExtensions
{
    /// <summary>
    /// Get the tenant ID from the current user's claims
    /// </summary>
    public static string? GetTenantId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("tenant_id")?.Value;
    }

}
