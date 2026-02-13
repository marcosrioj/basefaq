using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.EntityFramework.Tenant.Entities;
using BaseFaq.Tools.Seed.Abstractions;
using BaseFaq.Tools.Seed.Configuration;
using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Tenant.Enums;
using BaseFaq.Models.User.Enums;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Tools.Seed.Application;

public sealed class TenantSeedService : ITenantSeedService
{
    public bool HasData(TenantDbContext dbContext)
    {
        return dbContext.Tenants.Any() || dbContext.Users.Any() || dbContext.TenantConnections.Any();
    }

    public Guid Seed(TenantDbContext dbContext, TenantSeedRequest request, SeedCounts counts)
    {
        var existingExternalIds = dbContext.Users.AsNoTracking().Select(user => user.ExternalId).ToHashSet();
        var existingSlugs = dbContext.Tenants.AsNoTracking().Select(tenant => tenant.Slug).ToHashSet();

        var users = BuildUsers(counts.UserCount, existingExternalIds);
        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();

        var tenants = BuildSingleTenant(users, existingSlugs, request);
        dbContext.Tenants.AddRange(tenants);
        dbContext.SaveChanges();

        var connections = BuildSingleFaqConnection(request);
        dbContext.TenantConnections.AddRange(connections);
        dbContext.SaveChanges();

        var seedTenant = tenants.FirstOrDefault(t => t.App == AppEnum.Faq && t.IsActive)
                         ?? tenants.FirstOrDefault(t => t.App == AppEnum.Faq)
                         ?? tenants.First();

        return seedTenant.Id;
    }

    private static List<User> BuildUsers(int count, HashSet<string> existingExternalIds)
    {
        var users = new List<User>();
        var index = 1;

        while (users.Count < count)
        {
            var externalId = $"seed-user-{index:000}";
            if (existingExternalIds.Contains(externalId))
            {
                index++;
                continue;
            }

            users.Add(new User
            {
                Id = Guid.NewGuid(),
                GivenName = $"User {index:000}",
                SurName = index % 3 == 0 ? null : $"Seed {index:000}",
                Email = $"user{index:000}@seed.basefaq.local",
                ExternalId = externalId,
                PhoneNumber = index % 4 == 0 ? "" : $"+1-555-01{index:000}",
                Role = index % 7 == 0 ? UserRoleType.Admin : UserRoleType.Member
            });

            index++;
        }

        return users;
    }

    private static List<Tenant> BuildSingleTenant(
        IReadOnlyList<User> users,
        HashSet<string> existingSlugs,
        TenantSeedRequest request)
    {
        var slug = existingSlugs.Contains("tenant-001") ? $"tenant-{Guid.NewGuid():N}" : "tenant-001";
        var userId = users.Count > 0 ? users[0].Id : Guid.Empty;

        return
        [
            new Tenant
            {
                Id = Guid.NewGuid(),
                Slug = slug,
                Name = Tenant.DefaultTenantName,
                Edition = TenantEdition.Free,
                App = AppEnum.Faq,
                ConnectionString = request.FaqConnectionString,
                IsActive = true,
                UserId = userId
            }
        ];
    }

    private static List<TenantConnection> BuildSingleFaqConnection(TenantSeedRequest request)
    {
        return
        [
            new TenantConnection
            {
                Id = Guid.NewGuid(),
                ConnectionString = request.FaqConnectionString,
                App = AppEnum.Faq,
                IsCurrent = true
            }
        ];
    }
}