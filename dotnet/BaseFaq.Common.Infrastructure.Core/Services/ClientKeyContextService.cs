using System.Net;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Common.Infrastructure.Core.Constants;
using Microsoft.AspNetCore.Http;

namespace BaseFaq.Common.Infrastructure.Core.Services;

public sealed class ClientKeyContextService(IHttpContextAccessor httpContextAccessor) : IClientKeyContextService
{
    public string GetRequiredClientKey()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null ||
            !httpContext.Items.TryGetValue(ClientKeyContextKeys.ClientKeyItemKey, out var clientKeyValue) ||
            clientKeyValue is not string clientKey ||
            string.IsNullOrWhiteSpace(clientKey))
        {
            throw new ApiErrorException(
                "Client key context is missing from the current request.",
                errorCode: (int)HttpStatusCode.BadRequest);
        }

        return clientKey;
    }
}