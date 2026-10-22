namespace WeddingDay.ServiceDefaults;

public static class Constants
{
    public const string ProjectName = "WeddingDay";
    public const string ServiceNamespace = ProjectName;

    public static class ResourceNames
    {
        public const string Gateway = "gateway";
        public const string Postgres = "postgres";
        public const string PostgresDatabase = "weddingday-db";
        public const string Minio = "minio";
        public const string Redis = "redis";
        public const string RabbitMq = "rabbitmq";
        public const string Mailpit = "mailpit";
    }

    public static class VolumeNames
    {
        public const string Postgres = $"{ProjectName}-{ResourceNames.Postgres}-data";
        public const string Minio = $"{ProjectName}-{ResourceNames.Minio}-data";
        public const string Redis = $"{ProjectName}-{ResourceNames.Redis}-data";
        public const string RabbitMq = $"{ProjectName}-{ResourceNames.RabbitMq}-data";
        public const string Mailpit = $"{ProjectName}-{ResourceNames.Mailpit}-data";
    }

    public static class Ports
    {
        public const int Gateway = 5100;
        public const int GatewayHttps = 5101;

        public const int Postgres = 5432;
        public const int MinioApi = 9000;
        public const int MinioConsole = 9001;
        public const int Redis = 6379;
        public const int RabbitMq = 5672;
        public const int RabbitMqManagement = 15672;
        public const int MailpitSmtp = 1025;
        public const int MailpitHttp = 8025;
    }

    public static class EnvKeys
    {
        public const string PostgresUser = "POSTGRES_USER";
        public const string PostgresPassword = "POSTGRES_PASSWORD";
        public const string MinioRootUser = "MINIO_ROOT_USER";
        public const string MinioRootPassword = "MINIO_ROOT_PASSWORD";
        public const string RabbitMqUser = "RABBITMQ_DEFAULT_USER";
        public const string RabbitMqPassword = "RABBITMQ_DEFAULT_PASS";
        public const string RedisPassword = "REDIS_PASSWORD";

        public const string OidcAuthority = "OIDC_AUTHORITY";
        public const string OidcClientId = "OIDC_CLIENT_ID";
        public const string OidcClientSecret = "OIDC_CLIENT_SECRET";

        public const string OtlpEndpoint = "OTEL_EXPORTER_OTLP_ENDPOINT";
    }

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

    public static class ContainerImages
    {
        public static class Postgres
        {
            public const string Registry = "docker.io";
            public const string Image = "library/postgres";
            public const string Tag = "17";
            public const string DataPath = "/var/lib/postgresql/data";
        }

        public static class Minio
        {
            public const string Registry = "docker.io";
            public const string Image = "chainguard/minio";
            public const string Tag = "latest";
            public const string DataPath = "/data";
        }

        public static class Redis
        {
            public const string Registry = "docker.io";
            public const string Image = "library/redis";
            public const string Tag = "8";
            public const string DataPath = "/data";
        }

        public static class RabbitMq
        {
            public const string Registry = "docker.io";
            public const string Image = "library/rabbitmq";
            public const string Tag = "4-management";
            public const string DataPath = "/var/lib/rabbitmq";
        }

        public static class Mailpit
        {
            public const string Registry = "docker.io";
            public const string Image = "axllent/mailpit";
            public const string Tag = "v1.27";
            public const string DataPath = "/data";
        }
    }
}
