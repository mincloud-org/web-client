using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Web.Server.Services;


public class AccessTokenTransformProvider : ITransformProvider
{
    public void Apply(TransformBuilderContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.AddRequestTransform(async transformContext =>
        {
            var accessToken = await GetAccessTokenAsync(transformContext.HttpContext);
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Add the access token to the request url as a query parameter
                var uriBuilder = new UriBuilder(transformContext.HttpContext.Request.GetEncodedUrl());
                if (uriBuilder.Path.StartsWith("/hub", StringComparison.OrdinalIgnoreCase))
                {
                    var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                    query["access_token"] = accessToken;
                    uriBuilder.Query = query.ToString();
                    transformContext.ProxyRequest.RequestUri = uriBuilder.Uri;
                }
                else
                {
                    transformContext.ProxyRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
                }
            }
        });
    }
    private async Task<string> GetAccessTokenAsync(HttpContext httpContext)
    {
        return await httpContext.GetTokenAsync("access_token") ?? string.Empty;
    }
    public void ValidateCluster(TransformClusterValidationContext context)
    {

    }

    public void ValidateRoute(TransformRouteValidationContext context)
    {
    }
}
