using Microsoft.AspNetCore.HttpOverrides;
using MinCloud.Internal.SDK;
using Web.Server.Services;

namespace Web.Server.Extensions;

public static class HostExtensions
{

    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddDefaultServices()
            .AddDefaultControllers()
            .AddDefaultDataProtection(builder.Configuration.GetConnectionString("Redis"))
            .AddDefaultCorsPolicy()
            .AddDefaultHealthChecks()
            .AddMultiTenantSupport()  // Add multi-tenant support
            .AddDefaultAuthentication(builder.Configuration)
            .AddDefaultOpenApi(builder.Configuration)
            .AddDefaultUserContexts();

        var identitySection = builder.Configuration.GetSection("Identity");

        builder.Services.AddMinCloudInternalApiClient(
            baseUrl: builder.Configuration["InternalApiUrl"] ?? throw new ArgumentNullException("InternalApiUrl"),
            useApiGateway: bool.Parse(builder.Configuration["UseApiGateway"] ?? "false"),
            identityOptions: new MinCloudIdentityOptions(
                ClientId: identitySection.GetRequiredValue("ClientId"),
                ClientSecret: identitySection.GetRequiredValue("ClientSecret"),
                Authority: identitySection.GetRequiredValue("Url"),
                Scopes: identitySection.GetRequiredSection("Scopes").GetChildren().Select(c => c.Key!).ToArray()
                ));

        builder.Services.AddReverseProxy()
            .AddTransforms<AccessTokenTransformProvider>()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        return builder;
    }

    public static WebApplication UseApplicationService(this WebApplication app)
    {

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        app.UseDefaultFiles();
        app.MapStaticAssets();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseDefaultOpenApi(app.Configuration);
        }

        app.MapDefaultHealthChecks();

        app.MapControllers();
        app.MapGet("/api/about", () => new
        {
            Name = "Web.Server",
            Version = "1.0.0",
            MinCloudEnv = app.Configuration["MINCLOUD_ENV"],
            app.Environment.EnvironmentName
        });
        app.MapFallbackToFile("/index.html").RequireAuthorization();
        app.MapReverseProxy();
        return app;
    }
}
