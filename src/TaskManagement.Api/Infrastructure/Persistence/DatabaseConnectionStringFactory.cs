using Microsoft.Data.SqlClient;

namespace TaskManagement.Api.Infrastructure.Persistence;

public static class DatabaseConnectionStringFactory
{
    public static string Create(IConfiguration configuration)
    {
        var configuredConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration["DB_CONNECTION_STRING"];

        if (!string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            return configuredConnectionString;
        }

        var password = configuration["DB_PASSWORD"];
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Database configuration is missing. Set DB_PASSWORD or ConnectionStrings__DefaultConnection.");
        }

        var server = configuration["DB_SERVER"] ?? "localhost";
        var port = configuration["DB_PORT"];
        var builder = new SqlConnectionStringBuilder
        {
            // Omitting DB_PORT preserves local SQL Server protocol resolution
            // (for example Shared Memory or a named instance).
            DataSource = string.IsNullOrWhiteSpace(port) ? server : $"{server},{port}",
            InitialCatalog = configuration["DB_NAME"] ?? "TaskManagement",
            UserID = configuration["DB_USER"] ?? "sa",
            Password = password,
            TrustServerCertificate = GetBoolean(configuration, "DB_TRUST_SERVER_CERTIFICATE", true),
            Encrypt = GetBoolean(configuration, "DB_ENCRYPT", false)
        };

        return builder.ConnectionString;
    }

    private static bool GetBoolean(IConfiguration configuration, string key, bool defaultValue) =>
        bool.TryParse(configuration[key], out var value) ? value : defaultValue;
}
