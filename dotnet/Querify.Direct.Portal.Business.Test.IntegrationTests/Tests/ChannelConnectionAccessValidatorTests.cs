using Querify.Common.Architecture.Test.IntegrationTest.Shared.Tenancy;
using Querify.Common.EntityFramework.Tenant.Entities;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Direct.Portal.Business.Conversation.Rules;
using Querify.Models.Common.Enums;
using Querify.Models.Tenant.Enums;
using Xunit;
using TenantEntity = Querify.Common.EntityFramework.Tenant.Entities.Tenant;

namespace Querify.Direct.Portal.Business.Test.IntegrationTests.Tests;

public class ChannelConnectionAccessValidatorTests
{
    [Fact]
    public async Task Validate_AllowsConnectedConnectionOwnedBySelectedTenant()
    {
        var tenantId = Guid.NewGuid();
        using var context = TenantControlPlaneTestContext.Create(tenantId);
        var connection = await SeedConnectedConnectionAsync(context, tenantId);
        var validator = new ChannelConnectionAccessValidator(context.DbContext);

        await validator.ValidateAsync(tenantId, connection.Id, CancellationToken.None);
    }

    [Fact]
    public async Task Validate_RejectsConnectionOwnedByAnotherTenant()
    {
        var selectedTenantId = Guid.NewGuid();
        var owningTenantId = Guid.NewGuid();
        using var context = TenantControlPlaneTestContext.Create(selectedTenantId);
        await SeedTenantAsync(context, selectedTenantId);
        var connection = await SeedConnectedConnectionAsync(context, owningTenantId);
        var validator = new ChannelConnectionAccessValidator(context.DbContext);

        var exception = await Assert.ThrowsAsync<ApiErrorException>(() => validator.ValidateAsync(
            selectedTenantId,
            connection.Id,
            CancellationToken.None));

        Assert.Equal(422, exception.ErrorCode);
    }

    private static async Task<ChannelConnection> SeedConnectedConnectionAsync(
        TenantControlPlaneTestContext context,
        Guid tenantId)
    {
        await SeedTenantAsync(context, tenantId);
        var connection = new ChannelConnection
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = "Primary Direct connection",
            ProviderKey = $"direct-{tenantId:N}",
            Kind = ChannelConnectionKind.WhatsApp,
            ConnectionData = "{}",
            Status = ChannelConnectionStatus.Connected,
            IsEnabled = true
        };
        context.DbContext.ChannelConnections.Add(connection);
        await context.DbContext.SaveChangesAsync();
        return connection;
    }

    private static async Task SeedTenantAsync(TenantControlPlaneTestContext context, Guid tenantId)
    {
        if (await context.DbContext.Tenants.FindAsync(tenantId) is not null)
        {
            return;
        }

        context.DbContext.Tenants.Add(new TenantEntity
        {
            Id = tenantId,
            Slug = $"tenant-{tenantId:N}",
            Name = "Direct test tenant",
            Edition = TenantEdition.Free,
            Module = ModuleEnum.QnA,
            ConnectionString = IntegrationTestConnectionStrings.QnA,
            IsActive = true
        });
        await context.DbContext.SaveChangesAsync();
    }
}
