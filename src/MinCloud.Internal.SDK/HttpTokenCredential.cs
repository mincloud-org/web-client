using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace MinCloud.Internal.SDK;

public class HttpTokenCredential(IHttpContextAccessor httpContextAccessor) : ITokenCredential
{
    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        return accessToken;
    }
}
