namespace BaseFaq.Common.Infrastructure.Core.Abstractions;

public interface IUserIdProvider
{
    Guid GetUserId(string externalUserId);
}