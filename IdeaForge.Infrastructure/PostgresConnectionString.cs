using Microsoft.Extensions.Configuration;

namespace IdeaForge.Infrastructure;

/// <summary>
/// Resolves the Postgres connection string from configuration, accepting both
/// Npgsql key-value format and URL format (postgresql://user:pass@host:port/db?sslmode=require)
/// as provided by hosts like Neon and Render.
/// </summary>
public static class PostgresConnectionString
{
    public static string? Resolve(IConfiguration configuration)
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

        string? resolved = !string.IsNullOrWhiteSpace(databaseUrl)
            ? Normalize(databaseUrl)
            : configuration.GetConnectionString("DefaultConnection");

        if (Environment.GetEnvironmentVariable("REQUIRE_SSL") == "true" && resolved is not null)
            resolved = EnsureSslRequire(resolved);

        return resolved;
    }

    public static string? Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var trimmed = value.Trim();

        if (!trimmed.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !trimmed.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed;
        }

        var uri = new Uri(trimmed);

        var userInfo = uri.UserInfo.Split(':', 2);
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
        var database = Uri.UnescapeDataString(uri.AbsolutePath.Trim('/'));

        var query = ParseQuery(uri.Query);
        query.TryGetValue("sslmode", out var sslMode);
        var npgsqlSsl = (sslMode ?? "prefer").ToLowerInvariant() switch
        {
            "require" => "Require",
            "prefer" => "Prefer",
            "disable" => "Disable",
            "allow" => "Allow",
            "verify-ca" => "VerifyCA",
            "verify-full" => "VerifyFull",
            _ => "Prefer"
        };

        var parts = new List<string>
        {
            $"Host={uri.Host}",
            $"Port={(uri.Port == -1 ? 5432 : uri.Port)}",
            $"Username={username}",
            $"Password={password}",
            $"SSL Mode={npgsqlSsl}"
        };

        if (!string.IsNullOrEmpty(database))
            parts.Insert(2, $"Database={database}");

        return string.Join(";", parts);
    }

    private static string EnsureSslRequire(string connectionString)
    {
        if (connectionString.Contains("SSL Mode", StringComparison.OrdinalIgnoreCase))
            return connectionString;

        return connectionString.TrimEnd(';') + ";SSL Mode=Require";
    }

    private static Dictionary<string, string> ParseQuery(string query)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var pair in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = pair.Split('=', 2);
            if (kv.Length == 2)
                result[Uri.UnescapeDataString(kv[0])] = Uri.UnescapeDataString(kv[1]);
        }

        return result;
    }
}
