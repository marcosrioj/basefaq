using System.Net;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Models.Broadcast.Enums;
using ThreadEntity = Querify.Broadcast.Common.Domain.Entities.Thread;

namespace Querify.Broadcast.Common.Domain.BusinessRules.Threads;

public static class ThreadRules
{
    public static void EnsureSupportedStatus(ThreadStatus status)
    {
        if (status is ThreadStatus.Open or ThreadStatus.Closed)
            return;

        throw new ApiErrorException(
            "Unsupported thread status.",
            (int)HttpStatusCode.UnprocessableEntity);
    }

    public static void EnsureAcceptsItems(ThreadEntity thread)
    {
        if (thread.Status is ThreadStatus.Open)
            return;

        throw new ApiErrorException(
            "Closed threads cannot receive new items.",
            (int)HttpStatusCode.UnprocessableEntity);
    }
}
