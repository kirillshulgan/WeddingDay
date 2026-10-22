namespace WeddingDay.ServiceDefaults;

/// <summary>
/// Единый источник имён, версий, портов и ключей конфигурации проекта.
/// </summary>
public static class Constants
{
    #region Application

    public const string ProjectName = "WeddingDay";
    public const string ProjectNameToLower = "weddingday";
    public const string ServiceNamespace = ProjectName;

    #endregion

    #region Infrastructure: resources

    public static class ResourceNames
    {
        public const string Gateway = "gateway";
        public const string Postgres = "postgres";
        public const string PostgresDatabase = "weddingday-db";
        public const string Minio = "minio";
        public const string Redis = "redis";
        public const string RedisInsight = "redisinsight";
        public const string RabbitMq = "rabbitmq";
        public const string Mailpit = "mailpit";
        public const string Keycloak = "keycloak";
    }

    public static class VolumeNames
    {
        public const string Postgres = $"{ProjectNameToLower}-{ResourceNames.Postgres}-data";
        public const string Minio = $"{ProjectNameToLower}-{ResourceNames.Minio}-data";
        public const string Redis = $"{ProjectNameToLower}-{ResourceNames.Redis}-data";
        public const string RedisInsight = $"{ProjectNameToLower}-{ResourceNames.RedisInsight}-data";
        public const string RabbitMq = $"{ProjectNameToLower}-{ResourceNames.RabbitMq}-data";
        public const string Mailpit = $"{ProjectNameToLower}-{ResourceNames.Mailpit}-data";
        public const string Keycloak = $"{ProjectNameToLower}-{ResourceNames.Keycloak}-data";
    }

    public static class Ports
    {
        public const int Gateway = 5100;
        public const int GatewayHttps = 5101;

        public const int Postgres = 5432;
        public const int Redis = 6379;
        public const int RabbitMq = 5672;
        public const int RabbitMqManagement = 15672;
        public const int MinioApi = 9000;
        public const int MinioConsole = 9001;
        public const int MailpitSmtp = 1025;
        public const int MailpitHttp = 8025;
        public const int Keycloak = 8080;
    }

    public static class MinioEndpoints
    {
        public const string Api = "api";
        public const string Console = "console";
        public const string HealthPath = "/minio/health/live";
    }

    #endregion

    #region Infrastructure: container images

    public static class ContainerImages
    {
        public static class Postgres
        {
            public const string Registry = "docker.io";
            public const string Image = "library/postgres";
            public const string Tag = "18";
            public const string Reference = $"{Registry}/{Image}:{Tag}";
        }

        public static class Redis
        {
            public const string Registry = "docker.io";
            public const string Image = "library/redis";
            public const string Tag = "8";
            public const string DataPath = "/data";
            public const string Reference = $"{Registry}/{Image}:{Tag}";
        }

        public static class RabbitMq
        {
            public const string Registry = "docker.io";
            public const string Image = "library/rabbitmq";
            public const string Tag = "4-management";
            public const string DataPath = "/var/lib/rabbitmq";
            public const string Reference = $"{Registry}/{Image}:{Tag}";
        }

        public static class Minio
        {
            public const string Registry = "docker.io";
            public const string Image = "chainguard/minio";
            public const string Tag = "latest";
            public const string DataPath = "/data";
            public const string Reference = $"{Registry}/{Image}:{Tag}";
        }

        public static class Mailpit
        {
            public const string Registry = "docker.io";
            public const string Image = "axllent/mailpit";
            public const string Tag = "v1.27";
            public const string DataPath = "/data";
            public const string Reference = $"{Registry}/{Image}:{Tag}";
        }

        public static class Keycloak
        {
            public const string Registry = "quay.io";
            public const string Image = "keycloak/keycloak";
            public const string Tag = "26.7";
            public const string Reference = $"{Registry}/{Image}:{Tag}";
        }
    }

    #endregion

    #region Configuration and secrets

    public static class PathNames
    {
        public const string SourceRoot = "..";
        public const string SecretsDirectory = "secrets";
        public const string EnvFile = ".env";
        public const string ClientsDirectory = "clients";
        public const string RealmsDirectory = "Keycloak/Realms";
    }

    public static class EnvKeys
    {
        // PostgreSQL
        public const string PostgresUser = "POSTGRES_USER";
        public const string PostgresPassword = "POSTGRES_PASSWORD";

        // MinIO
        public const string MinioRootUser = "MINIO_ROOT_USER";
        public const string MinioRootPassword = "MINIO_ROOT_PASSWORD";

        // RabbitMQ
        public const string RabbitMqUser = "RABBITMQ_DEFAULT_USER";
        public const string RabbitMqPassword = "RABBITMQ_DEFAULT_PASS";

        // Redis
        public const string RedisPassword = "REDIS_PASSWORD";

        // OIDC
        public const string OidcAuthority = "OIDC_AUTHORITY";
        public const string OidcClientId = "OIDC_CLIENT_ID";
        public const string OidcClientSecret = "OIDC_CLIENT_SECRET";

        // OpenTelemetry
        public const string OtlpEndpoint = "OTEL_EXPORTER_OTLP_ENDPOINT";

        // Keycloak
        public const string KeycloakAdminUser = "KEYCLOAK_ADMIN";
        public const string KeycloakAdminPassword = "KEYCLOAK_ADMIN_PASSWORD";
    }

    public static class ParameterNames
    {
        public const string PostgresUser = "postgres-user";
        public const string PostgresPassword = "postgres-password";
        public const string MinioUser = "minio-user";
        public const string MinioPassword = "minio-password";
        public const string RabbitMqUser = "rabbitmq-user";
        public const string RabbitMqPassword = "rabbitmq-password";
        public const string RedisPassword = "redis-password";
        public const string OidcClientSecret = "oidc-client-secret";
        public const string KeycloakAdminUser = "keycloak-admin-user";
        public const string KeycloakAdminPassword = "keycloak-admin-password";
    }

    public static class OidcConfig
    {
        public const string Section = "Oidc";
        public const string Authority = "Authority";
        public const string ClientId = "ClientId";
        public const string ClientSecret = "ClientSecret";

        public const string AuthorityEnv = $"{Section}__{Authority}";
        public const string ClientIdEnv = $"{Section}__{ClientId}";
        public const string ClientSecretEnv = $"{Section}__{ClientSecret}";
    }

    public static class KeycloakConfig
    {
        public const string Realm = "weddingday";
        public const string GatewayClientId = "weddingday-gateway";
        public const string RolesClaim = "roles";
    }

    #endregion

    #region Observability

    public static class HealthPaths
    {
        public const string Ready = "/health";
        public const string Live = "/alive";
        public const string LiveTag = "live";
    }

    public static class TelemetryNames
    {
        public const string GatewayMeter = $"{ProjectName}.Gateway";
        public const string GatewayActivitySource = $"{ProjectName}.Gateway";

        public static readonly string[] FrameworkMeters =
        [
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel",
            "Microsoft.AspNetCore.Http.Connections",
            "Microsoft.AspNetCore.Routing",
            "Microsoft.AspNetCore.Diagnostics",
            "Microsoft.AspNetCore.RateLimiting",
            "System.Net.Http",
            "System.Net.NameResolution",
            "Yarp.ReverseProxy"
        ];

        public static readonly string[] AppMeters = [GatewayMeter];
        public static readonly string[] AppActivitySources = [GatewayActivitySource];
    }

    #endregion

    #region Clients

    public static class ClientApps
    {
        public sealed record ClientApp(string ResourceName, string DirectoryName);

        public static readonly ClientApp[] All =
        [
            new("guest-web", "guest-web"),
            new("admin-web", "admin-web")
        ];
    }

    #endregion
}
