using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace Web.Server.Services;


public class HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization == null)
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var accessToken = await httpContext.GetTokenAsync("access_token");
                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}