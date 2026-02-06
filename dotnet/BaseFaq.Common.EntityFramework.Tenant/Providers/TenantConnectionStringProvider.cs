using BaseFaq.Common.EntityFramework.Core.Abstractions;
using BaseFaq.Common.EntityFramework.Tenant.Security;
using Npgsql;

namespace BaseFaq.Common.EntityFramework.Tenant.Providers;

public sealed class TenantConnectionStringProvider : ITenantConnectionStringProvider
{
    public string GetConnectionString(Guid tenantId, string defaultConnectionString)
    {
        var encryptedConnectionString = GetEncryptedConnectionString(defaultConnectionString, tenantId);
        var decryptedConnectionString = StringCipher.Instance.Decrypt(encryptedConnectionString);

        if (string.IsNullOrWhiteSpace(decryptedConnectionString))
        {
            throw new InvalidOperationException(
                $"Tenant '{tenantId}' has an invalid connection string.");
        }

        return decryptedConnectionString;
    }

    private static string GetEncryptedConnectionString(string defaultConnectionString, Guid tenantId)
    {
        using var connection = new NpgsqlConnection(defaultConnectionString);
        using var command = connection.CreateCommand();

        command.CommandText = "SELECT \"ConnectionString\" FROM \"Tenants\" WHERE \"Id\" = @TenantId";
        command.Parameters.Add(new NpgsqlParameter("@TenantId", tenantId));

        connection.Open();
        var result = command.ExecuteScalar();

        if (result is not string connectionString || string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Tenant '{tenantId}' was not found or has an empty connection string.");
        }

        return connectionString;
    }
}