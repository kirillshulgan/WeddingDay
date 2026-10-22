using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using WeddingDay.Gateway.Logging;
using WeddingDay.Gateway.Observability;
using static WeddingDay.ServiceDefaults.Constants;

namespace WeddingDay.Gateway.Auth;

public sealed class RedisTicketStore(
    IDistributedCache cache,
    GatewayMetrics metrics,
    ILogger<RedisTicketStore> logger) : ITicketStore
{
    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = RedisKeys.TicketPrefix + WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        await RenewAsync(key, ticket);
        metrics.SessionOpened();
        logger.SessionCreated(ticket.Principal.Identity?.Name);
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = ticket.Properties.ExpiresUtc ?? DateTimeOffset.UtcNow.Add(ServiceDefaults.Constants.SessionDefaults.IdleTimeout)
        };
        await cache.SetAsync(key, TicketSerializer.Default.Serialize(ticket), options);
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var bytes = await cache.GetAsync(key);
        return bytes is null ? null : TicketSerializer.Default.Deserialize(bytes);
    }

    public async Task RemoveAsync(string key)
    {
        await cache.RemoveAsync(key);
        metrics.SessionClosed();
        logger.SessionRemoved();
    }
}
