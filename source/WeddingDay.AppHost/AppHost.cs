using WeddingDay.AppHost;
using static WeddingDay.ServiceDefaults.Constants;

var builder = DistributedApplication.CreateBuilder(args);

EnvFile.Load(builder.AppHostDirectory);

IResourceBuilder<ParameterResource> Secret(string parameterName, string envKey) =>
    builder.AddParameter(parameterName, EnvFile.Required(envKey), secret: true);

var postgresUser = Secret(ParameterNames.PostgresUser, EnvKeys.PostgresUser);
var postgresPassword = Secret(ParameterNames.PostgresPassword, EnvKeys.PostgresPassword);
var minioUser = Secret(ParameterNames.MinioUser, EnvKeys.MinioRootUser);
var minioPassword = Secret(ParameterNames.MinioPassword, EnvKeys.MinioRootPassword);
var rabbitUser = Secret(ParameterNames.RabbitMqUser, EnvKeys.RabbitMqUser);
var rabbitPassword = Secret(ParameterNames.RabbitMqPassword, EnvKeys.RabbitMqPassword);
var redisPassword = Secret(ParameterNames.RedisPassword, EnvKeys.RedisPassword);
var oidcClientSecret = Secret(ParameterNames.OidcClientSecret, EnvKeys.OidcClientSecret);

var postgres = builder.AddPostgres(ResourceNames.Postgres, postgresUser, postgresPassword, Ports.Postgres)
    .WithImageRegistry(ContainerImages.Postgres.Registry)
    .WithImage(ContainerImages.Postgres.Image)
    .WithImageTag(ContainerImages.Postgres.Tag)
    .WithDataVolume(VolumeNames.Postgres)
    .WithPgAdmin();

var weddingDb = postgres.AddDatabase(ResourceNames.PostgresDatabase);

var redis = builder.AddRedis(ResourceNames.Redis, Ports.Redis, redisPassword)
    .WithImageRegistry(ContainerImages.Redis.Registry)
    .WithImage(ContainerImages.Redis.Image)
    .WithImageTag(ContainerImages.Redis.Tag)
    .WithDataVolume(VolumeNames.Redis)
    .WithRedisInsight(insight => insight.WithDataVolume(VolumeNames.RedisInsight));

var rabbit = builder.AddRabbitMQ(ResourceNames.RabbitMq, rabbitUser, rabbitPassword, Ports.RabbitMq)
    .WithManagementPlugin(Ports.RabbitMqManagement)
    .WithImageRegistry(ContainerImages.RabbitMq.Registry)
    .WithImage(ContainerImages.RabbitMq.Image)
    .WithImageTag(ContainerImages.RabbitMq.Tag)
    .WithDataVolume(VolumeNames.RabbitMq);

var mailpit = builder.AddMailPit(ResourceNames.Mailpit, Ports.MailpitHttp, Ports.MailpitSmtp)
    .WithImageRegistry(ContainerImages.Mailpit.Registry)
    .WithImage(ContainerImages.Mailpit.Image)
    .WithImageTag(ContainerImages.Mailpit.Tag)
    .WithDataVolume(VolumeNames.Mailpit);

var minio = builder.AddContainer(ResourceNames.Minio, ContainerImages.Minio.Image, ContainerImages.Minio.Tag)
    .WithImageRegistry(ContainerImages.Minio.Registry)
    .WithEnvironment(EnvKeys.MinioRootUser, minioUser)
    .WithEnvironment(EnvKeys.MinioRootPassword, minioPassword)
    .WithArgs("server", ContainerImages.Minio.DataPath, "--console-address", $":{Ports.MinioConsole}")
    .WithHttpEndpoint(port: Ports.MinioApi, targetPort: Ports.MinioApi, name: MinioEndpoints.Api)
    .WithHttpEndpoint(port: Ports.MinioConsole, targetPort: Ports.MinioConsole, name: MinioEndpoints.Console)
    .WithVolume(VolumeNames.Minio, ContainerImages.Minio.DataPath)
    .WithHttpHealthCheck(MinioEndpoints.HealthPath, endpointName: MinioEndpoints.Api);

var gateway = builder.AddProject<Projects.WeddingDay_Gateway>(ResourceNames.Gateway)
    .WithReference(redis).WaitFor(redis)
    .WithEnvironment(OidcConfig.AuthorityEnv, EnvFile.Required(EnvKeys.OidcAuthority))
    .WithEnvironment(OidcConfig.ClientIdEnv, EnvFile.Required(EnvKeys.OidcClientId))
    .WithEnvironment(OidcConfig.ClientSecretEnv, oidcClientSecret)
    .WithExternalHttpEndpoints();

foreach (var client in ClientApps.All)
{
    var clientDirectory = Path.GetFullPath(Path.Combine(
        builder.AppHostDirectory,
        PathNames.SourceRoot,
        PathNames.ClientsDirectory,
        client.DirectoryName));

    if (!Directory.Exists(clientDirectory))
    {
        Console.WriteLine($"[AppHost] Клиент '{client.ResourceName}' пропущен: '{clientDirectory}' не найден.");
        continue;
    }

    var clientApp = builder.AddViteApp(client.ResourceName, clientDirectory);
    gateway.WithReference(clientApp).WaitFor(clientApp);
}

builder.Build().Run();
