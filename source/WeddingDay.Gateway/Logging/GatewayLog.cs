namespace WeddingDay.Gateway.Logging;

internal static partial class GatewayLog
{
    [LoggerMessage(EventId = 1000, Level = LogLevel.Information, Message = "Session created for {User}")]
    public static partial void SessionCreated(this ILogger logger, string? user);

    [LoggerMessage(EventId = 1001, Level = LogLevel.Information, Message = "Session removed")]
    public static partial void SessionRemoved(this ILogger logger);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Information, Message = "Login requested, redirect after login: {ReturnUrl}")]
    public static partial void LoginRequested(this ILogger logger, string returnUrl);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Information, Message = "Logout requested")]
    public static partial void LogoutRequested(this ILogger logger);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Warning, Message = "OIDC remote failure on {Path}")]
    public static partial void OidcRemoteFailure(this ILogger logger, Exception? exception, string? path);

    [LoggerMessage(EventId = 1005, Level = LogLevel.Warning, Message = "CSRF header missing on {Method} {Path}")]
    public static partial void CsrfRejected(this ILogger logger, string method, string? path);
}
