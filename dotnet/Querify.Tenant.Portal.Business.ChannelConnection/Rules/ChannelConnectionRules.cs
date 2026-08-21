using System.Net;
using System.Text.Json;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Models.Tenant.Enums;
using ChannelConnectionEntity = Querify.Common.EntityFramework.Tenant.Entities.ChannelConnection;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Rules;

internal static class ChannelConnectionRules
{
    public static void EnsureSupportedKind(ChannelConnectionKind kind)
    {
        if (Enum.IsDefined(kind))
            return;

        throw new ApiErrorException("Unsupported channel connection kind.", (int)HttpStatusCode.UnprocessableEntity);
    }

    public static string NormalizeConnectionData(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ApiErrorException("Connection data is required.", (int)HttpStatusCode.BadRequest);

        try
        {
            using var document = JsonDocument.Parse(value);
            if (document.RootElement.ValueKind is not JsonValueKind.Object)
                throw new ApiErrorException(
                    "Connection data must be a JSON object.",
                    (int)HttpStatusCode.BadRequest);

            var normalized = JsonSerializer.Serialize(document.RootElement);
            if (normalized.Length > ChannelConnectionEntity.MaxConnectionDataLength)
                throw new ApiErrorException(
                    $"Connection data cannot exceed {ChannelConnectionEntity.MaxConnectionDataLength} characters.",
                    (int)HttpStatusCode.BadRequest);

            return normalized;
        }
        catch (JsonException)
        {
            throw new ApiErrorException("Connection data must contain valid JSON.", (int)HttpStatusCode.BadRequest);
        }
    }

    public static void ResetOperationalState(ChannelConnectionEntity entity)
    {
        entity.Status = ChannelConnectionStatus.Pending;
        entity.CredentialsExpireAtUtc = null;
        entity.LastCredentialsRefreshAtUtc = null;
        entity.LastConnectedAtUtc = null;
        entity.LastSynchronizedAtUtc = null;
        entity.LastErrorAtUtc = null;
        entity.LastErrorMessage = null;
    }
}
