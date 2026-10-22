using System.Diagnostics.Metrics;
using static WeddingDay.ServiceDefaults.Constants;

namespace WeddingDay.Gateway.Observability;

public sealed class GatewayMetrics
{
    private readonly Counter<long> _logins;
    private readonly Counter<long> _logouts;
    private readonly Counter<long> _csrfRejected;
    private readonly Counter<long> _tokenRefresh;
    private readonly UpDownCounter<long> _activeSessions;

    public GatewayMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(TelemetryNames.GatewayMeter);

        _logins = meter.CreateCounter<long>(TelemetryNames.LoginsMetric, "{login}", "Completed login attempts.");
        _logouts = meter.CreateCounter<long>(TelemetryNames.LogoutsMetric, "{logout}", "Completed logouts.");
        _csrfRejected = meter.CreateCounter<long>(TelemetryNames.CsrfRejectedMetric, "{request}", "Requests rejected by CSRF protection.");
        _tokenRefresh = meter.CreateCounter<long>(TelemetryNames.TokenRefreshMetric, "{refresh}", "Access token refresh attempts.");
        _activeSessions = meter.CreateUpDownCounter<long>(TelemetryNames.ActiveSessionsMetric, "{session}", "Currently active BFF sessions.");
    }

    public void LoginCompleted(bool success) => _logins.Add(1, ResultTag(success));
    public void LogoutCompleted() => _logouts.Add(1);
    public void CsrfRejected() => _csrfRejected.Add(1);
    public void TokenRefreshed(bool success) => _tokenRefresh.Add(1, ResultTag(success));
    public void SessionOpened() => _activeSessions.Add(1);
    public void SessionClosed() => _activeSessions.Add(-1);

    private static KeyValuePair<string, object?> ResultTag(bool success) =>
        new(TelemetryNames.ResultTag, success ? TelemetryNames.SuccessValue : TelemetryNames.FailureValue);
}
