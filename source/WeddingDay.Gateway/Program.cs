using WeddingDay.Gateway.Auth;
using WeddingDay.Gateway.Endpoints;
using WeddingDay.Gateway.Observability;
using WeddingDay.ServiceDefaults;
using static WeddingDay.ServiceDefaults.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisDistributedCache(ResourceNames.Redis);

builder.Services.AddProblemDetails();
builder.Services.AddSingleton<GatewayMetrics>();
builder.AddGatewayAuthentication();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapBffEndpoints();

app.Run();
