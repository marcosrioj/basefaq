using Querify.Models.Common.Enums;
using Querify.Models.Tenant.Dtos.ChannelConnection;
using Querify.Models.Tenant.Enums;
using Querify.Tenant.Portal.Business.ChannelConnection.Commands.CreateChannelConnection;
using Querify.Tenant.Portal.Business.ChannelConnection.Queries.GetChannelConnection;
using Querify.Tenant.Portal.Business.ChannelConnection.Rules;
using Querify.Tenant.Portal.Business.Tenant.Service;
using Querify.Tenant.Portal.Test.IntegrationTests.Helpers;
using Xunit;

namespace Querify.Tenant.Portal.Test.IntegrationTests.Tests.ChannelConnection;

public class ChannelConnectionCommandQueryTests
{
    [Fact]
    public async Task CreateFromModuleTenant_StoresConnectionOnWorkspaceBaseTenant()
    {
        var userId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();
        var baseTenantId = Guid.NewGuid();
        var directTenantId = Guid.NewGuid();
        using var context = TestContext.Create(tenantId: directTenantId, userId: userId);
        await TestDataFactory.SeedTenantAsync(
            context.DbContext,
            id: baseTenantId,
            module: ModuleEnum.QnA,
            workspaceId: workspaceId,
            userId: userId);
        await TestDataFactory.SeedTenantAsync(
            context.DbContext,
            id: directTenantId,
            module: ModuleEnum.Direct,
            workspaceId: workspaceId,
            userId: userId);

        var accessService = new TenantPortalAccessService(context.DbContext, context.SessionService);
        var resolver = new ChannelConnectionTenantResolver(context.DbContext, accessService);
        var createHandler = new ChannelConnectionsCreateCommandHandler(
            context.DbContext,
            resolver,
            context.SessionService);
        var id = await createHandler.Handle(
            new ChannelConnectionsCreateCommand
            {
                TenantId = directTenantId,
                Request = new ChannelConnectionCreateRequestDto
                {
                    Name = "Primary Instagram",
                    ProviderKey = "instagram-primary",
                    Kind = ChannelConnectionKind.Instagram,
                    ConnectionData = "{\"accessToken\":\"secret\"}",
                    IsEnabled = true
                }
            },
            CancellationToken.None);

        context.DbContext.ChangeTracker.Clear();
        var getHandler = new ChannelConnectionsGetQueryHandler(context.DbContext, resolver);
        var result = await getHandler.Handle(
            new ChannelConnectionsGetQuery { TenantId = directTenantId, Id = id },
            CancellationToken.None);

        Assert.Equal(baseTenantId, result.TenantId);
        Assert.Equal("Primary Instagram", result.Name);
        Assert.Equal("instagram-primary", result.ProviderKey);
        Assert.Equal(ChannelConnectionKind.Instagram, result.Kind);
        Assert.Equal(ChannelConnectionStatus.Pending, result.Status);
        Assert.True(result.IsEnabled);
        Assert.Null(result.LastErrorMessage);
        Assert.NotNull(result.CreatedAtUtc);
    }
}
