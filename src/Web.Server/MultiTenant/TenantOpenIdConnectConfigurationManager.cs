using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Web.Server.MultiTenant;

/// <summary>
/// Tenant-aware OpenID Connect configuration manager
/// This manager dynamically loads OIDC configuration based on the current tenant
/// </summary>
public class TenantOpenIdConnectConfigurationManager : IConfigurationManager<OpenIdConnectConfiguration>
{
    private readonly ITenantResolver _tenantResolver;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseAuthority;
    private readonly Dictionary<string, ConfigurationManager<OpenIdConnectConfiguration>> _managers = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public TenantOpenIdConnectConfigurationManager(
        ITenantResolver tenantResolver,
        IHttpClientFactory httpClientFactory,
        string baseAuthority)
    {
        _tenantResolver = tenantResolver;
        _httpClientFactory = httpClientFactory;
        _baseAuthority = baseAuthority.TrimEnd('/');
    }

    public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel)
    {
        var tenant = await _tenantResolver.ResolveAsync(cancel);
        
        if (tenant == null)
        {
            // Fallback to base authority if no tenant found
            return await GetOrCreateManager("default", _baseAuthority).GetConfigurationAsync(cancel);
        }

        var tenantId = tenant.Id.ToString();
        var tenantAuthority = $"{_baseAuthority}/{tenantId}";
        
        return await GetOrCreateManager(tenantId, tenantAuthority).GetConfigurationAsync(cancel);
    }

    public void RequestRefresh()
    {
        foreach (var manager in _managers.Values)
        {
            manager.RequestRefresh();
        }
    }

    private ConfigurationManager<OpenIdConnectConfiguration> GetOrCreateManager(string key, string authority)
    {
        if (_managers.TryGetValue(key, out var manager))
        {
            return manager;
        }

        _lock.Wait();
        try
        {
            if (_managers.TryGetValue(key, out manager))
            {
                return manager;
            }

            var metadataAddress = $"{authority}/.well-known/openid-configuration";
            var httpClient = _httpClientFactory.CreateClient();
            
            manager = new ConfigurationManager<OpenIdConnectConfiguration>(
                metadataAddress,
                new OpenIdConnectConfigurationRetriever(),
                httpClient);

            _managers[key] = manager;
            return manager;
        }
        finally
        {
            _lock.Release();
        }
    }
}
