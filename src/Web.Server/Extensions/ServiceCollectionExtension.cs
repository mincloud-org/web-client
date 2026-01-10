using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using Web.Server.Filters;
using Web.Server.Helpers;
using Web.Server.MultiTenant;
using Web.Server.Services;

namespace Web.Server.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDefaultControllers(this IServiceCollection services)
    {
        services
        .AddControllers(options =>
        {
            options.Filters.Add<HttpGlobalExceptionFilter>();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            // add enum converter
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });
        return services;
    }

    public static IServiceCollection AddDefaultUserContexts(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IUserContext, HttpUserContext>();
        return services;
    }

    public static IServiceCollection AddDefaultDataProtection(this IServiceCollection services, string? redisConnection)
    {
        var dpBuilder = services.AddDataProtection(opts =>
             {
                 opts.ApplicationDiscriminator = "MinCloud.Web";
             });
        if (redisConnection is not null)
        {
            dpBuilder.PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(ConnectionStringHelper.ConvertRedisConnectionString(redisConnection)),
              "MinCloud.Web.DataProtection.Keys");
        }
        return services;
    }
    public static IServiceCollection AddDefaultCorsPolicy(this IServiceCollection services)
    {
        return services.AddCors(options =>
        {
            options.AddPolicy("allow_any",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });
    }

    public static IServiceCollection AddDefaultAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var identitySection = configuration.GetSection("Identity");
        var baseAuthority = identitySection["Url"] ?? throw new ArgumentNullException("Identity:Url");
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            // Use tenant-aware configuration manager
            var serviceProvider = services.BuildServiceProvider();
            var tenantResolver = serviceProvider.GetRequiredService<ITenantResolver>();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            
            options.ConfigurationManager = new TenantOpenIdConnectConfigurationManager(
                tenantResolver,
                httpClientFactory,
                baseAuthority);
            
            options.Authority = baseAuthority;
            options.ClientId = identitySection["ClientId"];
            options.ClientSecret = identitySection["ClientSecret"];
            options.ResponseType = "code";
            options.SaveTokens = true;
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            var scopes = identitySection.GetRequiredSection("Scopes").Get<IDictionary<string, string>>();
            if (scopes != null)
            {
                foreach (var scope in scopes)
                {
                    options.Scope.Add(scope.Key);
                }
            }
            options.GetClaimsFromUserInfoEndpoint = true;

            options.UseTokenLifetime = true;
            options.RefreshOnIssuerKeyNotFound = true;
            options.RefreshInterval = TimeSpan.FromMinutes(5);
            options.AutomaticRefreshInterval = TimeSpan.FromMinutes(30);
            options.Events = new OpenIdConnectEvents
            {
                OnTokenResponseReceived = context =>
                {
                    if (context.TokenEndpointResponse.Error is null)
                    {
                        context.Properties?.StoreTokens(context.TokenEndpointResponse.Parameters.Select(kvp => new AuthenticationToken
                        {
                            Name = kvp.Key,
                            Value = kvp.Value
                        }));
                    }
                    else
                    {
                        context.Fail(new Exception($"OpenID Connect error: {context.TokenEndpointResponse.Error} - {context.TokenEndpointResponse.ErrorDescription}"));

                    }
                    return Task.CompletedTask;
                }
            };
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = "mincloud.web.auth";
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.SlidingExpiration = true;
        });
        return services;
    }
    /// <summary>
    /// Add multi-tenant support
    /// </summary>
    public static IServiceCollection AddMultiTenantSupport(this IServiceCollection services)
    {
        // Ensure HttpContextAccessor is registered (required for tenant resolution)
        services.AddHttpContextAccessor();

        // Register tenant resolver
        services.AddSingleton<ITenantResolver, HostBasedTenantResolver>();

        return services;
    }

    public static IServiceCollection AddDefaultServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddTransient<ITenantService, TenantService>();
        return services;
    }
}
