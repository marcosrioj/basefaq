using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Tenant.Enums;
using BaseFaq.Tenant.Portal.Test.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BaseFaq.Tenant.Portal.Test.IntegrationTests.Tests.BusinessRules;

public class TenantEntityConstraintsTests
{
    [Fact]
    public async Task Tenants_RequireUniqueSlug()
    {
        using var context = TestContext.Create();

        var tenantA = new BaseFaq.Common.EntityFramework.Tenant.Entities.Tenant
        {
            Slug = "unique-slug",
            Name = "Tenant A",
            Edition = TenantEdition.Free,
            App = AppEnum.Faq,
            ConnectionString = "Host=host.docker.internal;Database=a;Username=tenant;Password=tenant;",
            IsActive = true
        };
        var tenantB = new BaseFaq.Common.EntityFramework.Tenant.Entities.Tenant
        {
            Slug = "unique-slug",
            Name = "Tenant B",
            Edition = TenantEdition.Free,
            App = AppEnum.Faq,
            ConnectionString = "Host=host.docker.internal;Database=b;Username=tenant;Password=tenant;",
            IsActive = true
        };

        context.DbContext.Tenants.Add(tenantA);
        await context.DbContext.SaveChangesAsync();

        context.DbContext.Tenants.Add(tenantB);
        await Assert.ThrowsAsync<DbUpdateException>(() => context.DbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task Tenants_RequireUniqueUserId()
    {
        using var context = TestContext.Create();
        var userId = Guid.NewGuid();

        var tenantA = new BaseFaq.Common.EntityFramework.Tenant.Entities.Tenant
        {
            Slug = "tenant-a",
            Name = "Tenant A",
            Edition = TenantEdition.Free,
            App = AppEnum.Faq,
            ConnectionString = "Host=host.docker.internal;Database=a;Username=tenant;Password=tenant;",
            IsActive = true,
            UserId = userId
        };
        var tenantB = new BaseFaq.Common.EntityFramework.Tenant.Entities.Tenant
        {
            Slug = "tenant-b",
            Name = "Tenant B",
            Edition = TenantEdition.Free,
            App = AppEnum.Faq,
            ConnectionString = "Host=host.docker.internal;Database=b;Username=tenant;Password=tenant;",
            IsActive = true,
            UserId = userId
        };

        context.DbContext.Tenants.Add(tenantA);
        await context.DbContext.SaveChangesAsync();

        context.DbContext.Tenants.Add(tenantB);
        await Assert.ThrowsAsync<DbUpdateException>(() => context.DbContext.SaveChangesAsync());
    }
}