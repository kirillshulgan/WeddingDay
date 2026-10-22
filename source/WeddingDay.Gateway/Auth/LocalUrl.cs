namespace WeddingDay.Gateway.Auth;

public static class LocalUrl
{
    public static string Sanitize(string? url, string fallback = "/") =>
        !string.IsNullOrEmpty(url) &&
        url[0] == '/' &&
        (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))
            ? url
            : fallback;
}
