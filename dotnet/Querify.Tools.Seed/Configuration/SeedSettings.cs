using Microsoft.Extensions.Configuration;

namespace Querify.Tools.Seed.Configuration;

public sealed record SeedSettings(
    string TenantConnectionString,
    string QnAConnectionString,
    string DirectConnectionString,
    string BroadcastConnectionString)
{
    public static SeedSettings From(IConfiguration configuration)
    {
        var tenant = GetRequiredConnectionString(configuration, "TenantDb");
        var qna = GetRequiredConnectionString(configuration, "QnADb");
        var direct = GetRequiredConnectionString(configuration, "DirectDb");
        var broadcast = GetRequiredConnectionString(configuration, "BroadcastDb");
        return new SeedSettings(tenant, qna, direct, broadcast);
    }

    private static string GetRequiredConnectionString(IConfiguration configuration, string name)
    {
        var value = configuration.GetConnectionString(name);
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        throw new InvalidOperationException($"Missing connection string '{name}'. Set ConnectionStrings:{name}.");
    }
}
