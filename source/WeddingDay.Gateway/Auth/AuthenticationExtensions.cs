using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using StackExchange.Redis;
using System.Globalization;
using WeddingDay.Gateway.Logging;
using WeddingDay.Gateway.Observability;
using static WeddingDay.ServiceDefaults.Constants;
using SessionDefaults = WeddingDay.ServiceDefaults.Constants.SessionDefaults;

namespace WeddingDay.Gateway.Auth;

public static class AuthenticationExtensions
{
    public static WebApplicationBuilder AddGatewayAuthentication(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddOptions<OidcOptions>()
            .BindConfiguration(OidcConfig.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDataProtection().SetApplicationName(ProjectName);
        services.AddOptions<KeyManagementOptions>()
            .Configure<IConnectionMultiplexer>((options, redis) =>
                options.XmlRepository = new RedisXmlRepository(() => redis.GetDatabase(), RedisKeys.DataProtection));

        services.AddSingleton<RedisTicketStore>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, ConfigureCookie)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, _ => { });

        services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
            .Configure<RedisTicketStore>((options, store) => options.SessionStore = store);

        services.AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
            .Configure<IOptions<OidcOptions>, GatewayMetrics, IHostEnvironment>(ConfigureOidc);

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.AnyRole, p => p.RequireRole(Roles.All))
            .AddPolicy(Policies.Staff, p => p.RequireRole(Roles.Staff))
            .AddPolicy(Policies.AdminOnly, p => p.RequireRole(Roles.Admin));

        return builder;
    }

    private static void ConfigureCookie(CookieAuthenticationOptions options)
    {
        options.Cookie.Name = AuthNames.SessionCookie;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Path = "/";
        options.ExpireTimeSpan = SessionDefaults.IdleTimeout;
        options.SlidingExpiration = true;
        options.LoginPath = BffPaths.Login;
        options.ReturnUrlParameter = BffQuery.ReturnUrl;

        options.Events.OnSigningIn = ctx =>
        {
            ctx.Properties.Items[AuthNames.SessionStartItem] =
                DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture);
            return Task.CompletedTask;
        };

        options.Events.OnValidatePrincipal = async ctx =>
        {
            if (ctx.Properties.Items.TryGetValue(AuthNames.SessionStartItem, out var raw) &&
                DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, out var start) &&
                DateTimeOffset.UtcNow - start > SessionDefaults.AbsoluteTimeout)
            {
                ctx.RejectPrincipal();
                await ctx.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        };

        options.Events.OnRedirectToLogin = ctx => RespondOrRedirect(ctx, StatusCodes.Status401Unauthorized);
        options.Events.OnRedirectToAccessDenied = ctx => RespondOrRedirect(ctx, StatusCodes.Status403Forbidden);
    }

    private static Task RespondOrRedirect(RedirectContext<CookieAuthenticationOptions> ctx, int statusCode)
    {
        var path = ctx.Request.Path;
        if (path.StartsWithSegments(BffPaths.Api) || path.StartsWithSegments(BffPaths.User))
        {
            ctx.Response.StatusCode = statusCode;
        }
        else
        {
            ctx.Response.Redirect(ctx.RedirectUri);
        }

        return Task.CompletedTask;
    }

    private static void ConfigureOidc(
        OpenIdConnectOptions options,
        IOptions<OidcOptions> oidc,
        GatewayMetrics metrics,
        IHostEnvironment environment)
    {
        var settings = oidc.Value;

        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = settings.Authority;
        options.ClientId = settings.ClientId;
        options.ClientSecret = settings.ClientSecret;
        options.RequireHttpsMetadata = !environment.IsDevelopment();

        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ResponseMode = OpenIdConnectResponseMode.Query;
        options.UsePkce = true;
        options.SaveTokens = true;
        options.MapInboundClaims = false;
        options.GetClaimsFromUserInfoEndpoint = false;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.TokenValidationParameters.NameClaimType = ClaimNames.PreferredUsername;
        options.TokenValidationParameters.RoleClaimType = AuthNames.RolesClaimType;

        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = _ =>
            {
                metrics.LoginCompleted(success: true);
                return Task.CompletedTask;
            },
            OnRemoteFailure = ctx =>
            {
                metrics.LoginCompleted(success: false);
                ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("WeddingDay.Gateway.Auth")
                    .OidcRemoteFailure(ctx.Failure, ctx.Request.Path.Value);

                ctx.Response.Redirect($"/?{BffQuery.Error}={BffQuery.LoginFailed}");
                ctx.HandleResponse();
                return Task.CompletedTask;
            },
            OnSignedOutCallbackRedirect = _ =>
            {
                metrics.LogoutCompleted();
                return Task.CompletedTask;
            }
        };
    }
}
