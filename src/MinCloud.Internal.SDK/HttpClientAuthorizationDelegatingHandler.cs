using System.Net.Http.Headers;

namespace MinCloud.Internal.SDK;

public class HttpClientAuthorizationDelegatingHandler(ITokenCredential tokenCredential) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization == null)
        {
            var accessToken = await tokenCredential.GetUserTokenAsync(cancellationToken);
            if (accessToken is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}