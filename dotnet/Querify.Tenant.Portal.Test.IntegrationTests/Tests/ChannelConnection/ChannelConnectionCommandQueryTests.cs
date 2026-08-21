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
    public async Task Create_StoresConnectionOnSelectedTenant()
    {
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        using var context = TestContext.Create(tenantId: tenantId, userId: userId);
        await TestDataFactory.SeedTenantAsync(
            context.DbContext,
            id: tenantId,
            userId: userId);

        var accessService = new TenantPortalAccessService(context.DbContext, context.SessionService);
        var resolver = new ChannelConnectionTenantResolver(accessService);
        var createHandler = new ChannelConnectionsCreateCommandHandler(
            context.DbContext,
            resolver,
            context.SessionService);
        var id = await createHandler.Handle(
            new ChannelConnectionsCreateCommand
            {
                TenantId = tenantId,
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
            new ChannelConnectionsGetQuery { TenantId = tenantId, Id = id },
            CancellationToken.None);

        Assert.Equal(tenantId, result.TenantId);
        Assert.Equal("Primary Instagram", result.Name);
        Assert.Equal("instagram-primary", result.ProviderKey);
        Assert.Equal(ChannelConnectionKind.Instagram, result.Kind);
        Assert.Equal(ChannelConnectionStatus.Pending, result.Status);
        Assert.True(result.IsEnabled);
        Assert.Null(result.LastErrorMessage);
        Assert.NotNull(result.CreatedAtUtc);
    }
}
