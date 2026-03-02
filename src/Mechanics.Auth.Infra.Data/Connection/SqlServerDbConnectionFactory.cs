using Mechanics.Auth.Infra.SecretProvider;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text.Json;

namespace Mechanics.Auth.Infra.Data.Connection;

public class SqlServerDbConnectionFactory(ILogger<SqlServerDbConnectionFactory> logger, ISecretProvider secretProvider)
    : IDbConnectionFactory
{
    private Task<string>? _connectionString;

    public async Task<IDbConnection> CreateConnection()
    {
        logger.LogDebug("Creating SQL connection");
        if (_connectionString is not null)
            return new SqlConnection(await _connectionString);

        await LoadConnectionString();
        return new SqlConnection(await _connectionString!);
    }

    private async Task LoadConnectionString()
    {
        logger.LogDebug("Loading connection string from Secret Manager");

        _connectionString = await secretProvider.GetDbConnectionString().ContinueWith(async resultTask =>
        {
            var connectionStringJson = await resultTask;
            using var jsonDocument = JsonDocument.Parse(connectionStringJson);

            var value = jsonDocument.RootElement.GetProperty("value").GetString();
            if (value == null)
                throw new InvalidOperationException("Connection string is empty.");

            logger.LogInformation("Connection string loaded");
            return value;
        });
    }
}
