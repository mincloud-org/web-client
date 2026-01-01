using System;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Web.Server.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddDefaultHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
         // Add a default liveness check to ensure app is responsive
         .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
        return services;
    }
    public static void MapDefaultHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health");

        // Only health checks tagged with the "live" tag must pass for app to be considered alive
        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });
    }
}
