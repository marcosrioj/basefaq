using Microsoft.EntityFrameworkCore;
using Querify.Broadcast.Portal.Business.Item.Commands.CreateItem;
using Querify.Broadcast.Portal.Business.Test.IntegrationTests.Helpers;
using Querify.Broadcast.Portal.Business.Thread.Queries.GetThread;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Models.Broadcast.Dtos.Item;
using Querify.Models.Broadcast.Enums;
using Xunit;
using BroadcastItem = Querify.Broadcast.Common.Domain.Entities.Item;
using BroadcastThread = Querify.Broadcast.Common.Domain.Entities.Thread;

namespace Querify.Broadcast.Portal.Business.Test.IntegrationTests.Tests;

public class BroadcastWorkflowTests
{
    [Fact]
    public async Task GetThread_ProjectsItemsAndLatestCaptureTime()
    {
        using var context = TestContext.Create();
        var capturedAtUtc = new DateTime(2026, 8, 21, 12, 30, 0, DateTimeKind.Utc);
        var thread = await SeedThreadAsync(context, ThreadStatus.Open);
        context.DbContext.Items.Add(new BroadcastItem
        {
            TenantId = context.SessionService.TenantId,
            ThreadId = thread.Id,
            Thread = thread,
            Kind = ItemKind.Comment,
            ActorKind = ActorKind.ExternalUser,
            Body = "Can you share the launch date?",
            CapturedAtUtc = capturedAtUtc
        });
        await context.DbContext.SaveChangesAsync();

        context.DbContext.ChangeTracker.Clear();
        var handler = new ThreadsGetQueryHandler(context.DbContext, context.SessionService);
        var result = await handler.Handle(new ThreadsGetQuery { Id = thread.Id }, CancellationToken.None);

        Assert.Equal(context.SessionService.TenantId, result.TenantId);
        Assert.Equal(thread.ChannelConnectionId, result.ChannelConnectionId);
        Assert.Equal(thread.Title, result.Title);
        Assert.Equal(ThreadStatus.Open, result.Status);
        Assert.Equal(1, result.ItemCount);
        Assert.Equal(capturedAtUtc, result.LastItemAtUtc);
        Assert.NotNull(result.CreatedAtUtc);
    }

    [Fact]
    public async Task CreateItem_RejectsClosedThread()
    {
        using var context = TestContext.Create();
        var thread = await SeedThreadAsync(context, ThreadStatus.Closed);
        var handler = new ItemsCreateCommandHandler(context.DbContext, context.SessionService);

        var exception = await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(
            new ItemsCreateCommand
            {
                Request = new ItemCreateRequestDto
                {
                    ThreadId = thread.Id,
                    Kind = ItemKind.Post,
                    ActorKind = ActorKind.Brand,
                    Body = "This item must not be appended.",
                    CapturedAtUtc = DateTime.UtcNow
                }
            },
            CancellationToken.None));

        Assert.Equal(422, exception.ErrorCode);
        Assert.Empty(await context.DbContext.Items.AsNoTracking().ToListAsync());
    }

    [Fact]
    public async Task SaveItem_RejectsThreadFromAnotherTenant()
    {
        using var context = TestContext.Create();
        var thread = await SeedThreadAsync(context, ThreadStatus.Open);
        context.DbContext.Items.Add(new BroadcastItem
        {
            TenantId = Guid.NewGuid(),
            ThreadId = thread.Id,
            Thread = thread,
            Kind = ItemKind.SharedMessage,
            ActorKind = ActorKind.System,
            Body = "Cross-tenant capture",
            CapturedAtUtc = DateTime.UtcNow
        });

        await Assert.ThrowsAsync<ApiErrorException>(() => context.DbContext.SaveChangesAsync());
    }

    private static async Task<BroadcastThread> SeedThreadAsync(TestContext context, ThreadStatus status)
    {
        var thread = new BroadcastThread
        {
            TenantId = context.SessionService.TenantId,
            ChannelConnectionId = Guid.NewGuid(),
            Title = "Product launch discussion",
            Status = status
        };
        context.DbContext.Threads.Add(thread);
        await context.DbContext.SaveChangesAsync();
        return thread;
    }
}
