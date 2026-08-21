using System.Net;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Direct.Common.Domain.Entities;
using Querify.Models.Direct.Enums;

namespace Querify.Direct.Common.Domain.BusinessRules.Conversations;

public static class ConversationRules
{
    public static void EnsureSupportedStatus(ConversationStatus status)
    {
        if (status is ConversationStatus.Open or ConversationStatus.Closed)
            return;

        throw new ApiErrorException(
            "Unsupported conversation status.",
            (int)HttpStatusCode.UnprocessableEntity);
    }

    public static void EnsureAcceptsMessages(Conversation conversation)
    {
        if (conversation.Status is ConversationStatus.Open)
            return;

        throw new ApiErrorException(
            "Closed conversations cannot receive new messages.",
            (int)HttpStatusCode.UnprocessableEntity);
    }
}
