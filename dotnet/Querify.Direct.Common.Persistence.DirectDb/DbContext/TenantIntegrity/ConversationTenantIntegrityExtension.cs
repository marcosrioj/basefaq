using Microsoft.EntityFrameworkCore;
using Querify.Common.EntityFramework.Core.Tenant.DbContext.TenantIntegrity;
using Querify.Direct.Common.Domain.Entities;

namespace Querify.Direct.Common.Persistence.DirectDb.DbContext.TenantIntegrity;

internal static class ConversationTenantIntegrityExtension
{
    internal static void EnsureConversationTenantIntegrity(
        this DirectDbContext dbContext,
        TenantIntegrityLookupCacheBase cacheBase)
    {
        Dictionary<Guid, Guid>? contactTenants = null;

        foreach (var entry in dbContext.ChangeTracker.Entries<Conversation>()
                     .Where(entry => entry.State is EntityState.Added or EntityState.Modified))
        {
            var conversation = entry.Entity;
            TenantIntegrityGuard.EnsureTenantMatch(
                conversation.TenantId,
                cacheBase.GetTenant<Contact>(conversation.ContactId, ref contactTenants),
                nameof(Conversation.ContactId));
        }
    }
}
