using System.ComponentModel.DataAnnotations;

namespace WeddingDay.Gateway.Auth;

public sealed class OidcOptions
{
    [Required, Url]
    public string Authority { get; init; } = string.Empty;

    [Required]
    public string ClientId { get; init; } = string.Empty;

    [Required]
    public string ClientSecret { get; init; } = string.Empty;
}
