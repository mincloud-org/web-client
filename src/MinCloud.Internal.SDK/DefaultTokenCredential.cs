using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MinCloud.Internal.SDK;

public class DefaultTokenCredential(
    IOptions<MinCloudApiOptions> options,
    IHttpContextAccessor httpContextAccessor,
    HttpClient httpClient) : ITokenCredential
{
    public async Task<string?> GetUserTokenAsync(CancellationToken cancellationToken = default)
    {
        var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        return accessToken;
    }

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var identityOptions = options.Value.IdentityOptions;
        if (identityOptions is null)
        {
            return null;
        }

        var disco = await httpClient.GetDiscoveryDocumentAsync(identityOptions.Authority, cancellationToken);
        if (disco.IsError) throw new Exception(disco.Error);

        // client credentials
        var token = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = identityOptions.ClientId,
            ClientSecret = identityOptions.ClientSecret,
            Scope = string.Join(" ", identityOptions.Scopes)
        });

        return token.AccessToken;
    }
}
