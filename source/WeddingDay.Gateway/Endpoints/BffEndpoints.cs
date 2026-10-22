using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeddingDay.Gateway.Auth;
using WeddingDay.Gateway.Logging;
using static WeddingDay.ServiceDefaults.Constants;

namespace WeddingDay.Gateway.Endpoints;

public sealed record UserInfo(string? UserName, string? DisplayName, string? Email, string[] Roles);

public static class BffEndpoints
{
    public static IEndpointRouteBuilder MapBffEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(BffPaths.Login, (
                [FromQuery(Name = BffQuery.ReturnUrl)] string? returnUrl,
                ILogger<Program> logger) =>
        {
            var target = LocalUrl.Sanitize(returnUrl);
            logger.LoginRequested(target);
            return Results.Challenge(
                new AuthenticationProperties { RedirectUri = target },
                [OpenIdConnectDefaults.AuthenticationScheme]);
        })
            .AllowAnonymous()
            .WithTags("BFF")
            .WithSummary("Starts the OIDC login flow.");

        app.MapGet(BffPaths.Logout, (
                [FromQuery(Name = BffQuery.ReturnUrl)] string? returnUrl,
                ILogger<Program> logger) =>
        {
            logger.LogoutRequested();
            return Results.SignOut(
                new AuthenticationProperties { RedirectUri = LocalUrl.Sanitize(returnUrl) },
                [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]);
        })
            .AllowAnonymous()
            .WithTags("BFF")
            .WithSummary("Ends the local session and the identity provider session.");

        app.MapGet(BffPaths.User, (ClaimsPrincipal user) => Results.Ok(new UserInfo(
                user.FindFirstValue(ClaimNames.PreferredUsername),
                user.FindFirstValue(ClaimNames.DisplayName),
                user.FindFirstValue(ClaimNames.Email),
                user.FindAll(ClaimNames.Roles).Select(c => c.Value).ToArray())))
            .RequireAuthorization()
            .WithTags("BFF")
            .WithSummary("Returns the current user, or 401 when there is no session.")
            .Produces<UserInfo>()
            .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
