using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.EntityFramework.Tenant.Entities;
using BaseFaq.Models.Common.Enums;
using BaseFaq.Models.Tenant.Enums;
using BaseFaq.Models.User.Enums;

namespace BaseFaq.Tenant.BackOffice.Test.IntegrationTests.Helpers;

public static class TestDataFactory
{
    public static async Task<Common.EntityFramework.Tenant.Entities.Tenant> SeedTenantAsync(
        TenantDbContext dbContext,
        string? slug = null,
        string? name = null,
        TenantEdition edition = TenantEdition.Free,
        AppEnum app = AppEnum.Faq,
        string? connectionString = null,
        bool isActive = true,
        Guid? userId = null)
    {
        var tenant = new Common.EntityFramework.Tenant.Entities.Tenant
        {
            Slug = slug ?? $"tenant-{Guid.NewGuid():N}",
            Name = name ?? "Default Tenant",
            Edition = edition,
            App = app,
            ConnectionString = connectionString ??
                               "Host=host.docker.internal;Database=tenant;Username=tenant;Password=tenant;",
            IsActive = isActive,
            UserId = userId
        };

        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync();
        return tenant;
    }

    public static async Task<TenantConnection> SeedTenantConnectionAsync(
        TenantDbContext dbContext,
        AppEnum app = AppEnum.Faq,
        string? connectionString = null,
        bool isCurrent = true)
    {
        var tenantConnection = new TenantConnection
        {
            App = app,
            ConnectionString = connectionString ??
                               "Host=host.docker.internal;Database=tenant;Username=tenant;Password=tenant;",
            IsCurrent = isCurrent
        };

        dbContext.TenantConnections.Add(tenantConnection);
        await dbContext.SaveChangesAsync();
        return tenantConnection;
    }

    public static async Task<User> SeedUserAsync(
        TenantDbContext dbContext,
        string? givenName = null,
        string? surName = null,
        string? email = null,
        string? externalId = null,
        string? phoneNumber = null,
        UserRoleType role = UserRoleType.Member)
    {
        var user = new User
        {
            GivenName = givenName ?? "Jordan",
            SurName = surName,
            Email = email ?? $"{Guid.NewGuid():N}@example.test",
            ExternalId = externalId ?? $"ext-{Guid.NewGuid():N}",
            PhoneNumber = phoneNumber ?? "555-0000",
            Role = role
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }
}