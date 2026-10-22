using DotNetEnv;
using static WeddingDay.ServiceDefaults.Constants;

namespace WeddingDay.AppHost;

internal static class EnvFile
{
    public static void Load(string appHostDirectory)
    {
        var path = Path.GetFullPath(Path.Combine(
            appHostDirectory,
            PathNames.SourceRoot,
            PathNames.SecretsDirectory,
            PathNames.EnvFile));

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $".env файл не найден. Скопируйте файл '{PathNames.EnvFile}.example' в файл '{PathNames.EnvFile}' в каталоге '{Path.GetDirectoryName(path)}'.",
                path);
        }

        Env.Load(path);
    }

    public static string Required(string key)
    {
        var value = Env.GetString(key);
        return string.IsNullOrWhiteSpace(value)
            ? throw new InvalidOperationException($"Обязательная переменная '{key}' в .env файле пустая или отсутствует.")
            : value;
    }
}
