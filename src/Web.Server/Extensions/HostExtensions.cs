using System.Text.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Web.Server.Services;

namespace Web.Server.Extensions;

public static class HostExtensions
{

    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddDefaultControllers()
            .AddDefaultDataProtection(builder.Configuration.GetConnectionString("Redis"))
            .AddDefaultCorsPolicy()
            .AddDefaultHealthChecks()
            .AddDefaultAuthentication(builder.Configuration)
            .AddDefaultOpenApi(builder.Configuration)
            .AddDefaultUserContexts();

        builder.Services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
        // .AddDeepinApiClient(options =>
        // {
        //     options.BaseUrl = builder.Configuration["DeepinApiUrl"] ?? throw new ArgumentNullException("DeepinApiUrl");
        //     options.Timeout = TimeSpan.FromSeconds(30);
        //     options.JsonSerializerOptions = new JsonSerializerOptions
        //     {
        //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //         PropertyNameCaseInsensitive = true,
        //         Converters =
        //         {
        //             new System.Text.Json.Serialization.JsonStringEnumConverter()
        //         },
        //         DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        //         WriteIndented = true
        //     };
        // }).AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

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
