using System.Net;
using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Business.Vote.Commands.CreateVote;
using BaseFaq.Faq.FaqWeb.Business.Vote.Commands.UpdateVote;
using BaseFaq.Faq.FaqWeb.Business.Vote.Helpers;
using BaseFaq.Faq.FaqWeb.Business.Vote.Queries.GetVote;
using BaseFaq.Faq.FaqWeb.Business.Vote.Queries.GetVoteList;
using BaseFaq.Faq.FaqWeb.Test.IntegrationTests.Helpers;
using BaseFaq.Models.Faq.Dtos.Vote;
using BaseFaq.Models.Faq.Enums;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace BaseFaq.Faq.FaqWeb.Test.IntegrationTests.Tests.Vote;

public class VoteCommandQueryTests
{
    [Fact]
    public async Task CreateVote_PersistsEntityAndReturnsId()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("192.0.2.44");
        httpContext.Request.Headers.UserAgent = "TestAgent/1.0";

        using var context = TestContext.Create(httpContext: httpContext);
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var faqItem = await TestDataFactory.SeedFaqItemAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id);

        var handler = new VotesCreateVoteCommandHandler(
            context.DbContext,
            context.SessionService,
            context.HttpContextAccessor);
        var request = new VotesCreateVoteCommand
        {
            Like = true,
            UnLikeReason = null,
            FaqItemId = faqItem.Id
        };

        var identity = VoteRequestContext.GetIdentity(context.SessionService, context.HttpContextAccessor);
        var id = await handler.Handle(request, CancellationToken.None);

        var vote = await context.DbContext.Votes.FindAsync(id);
        Assert.NotNull(vote);
        Assert.True(vote!.Like);
        Assert.Equal(identity.UserPrint, vote.UserPrint);
        Assert.Equal(identity.Ip, vote.Ip);
        Assert.Equal(identity.UserAgent, vote.UserAgent);
        Assert.Equal(faqItem.Id, vote.FaqItemId);
        Assert.Equal(context.SessionService.TenantId, vote.TenantId);
    }

    [Fact]
    public async Task CreateVote_ThrowsWhenUnLikeReasonMissing()
    {
        using var context = TestContext.Create(httpContext: new DefaultHttpContext());
        var handler = new VotesCreateVoteCommandHandler(
            context.DbContext,
            context.SessionService,
            context.HttpContextAccessor);
        var request = new VotesCreateVoteCommand
        {
            Like = false,
            UnLikeReason = null,
            FaqItemId = Guid.NewGuid()
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(422, exception.ErrorCode);
    }

    [Fact]
    public async Task CreateVote_UsesAuthenticatedUserIdForUserPrint()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("192.0.2.100");
        httpContext.Request.Headers.UserAgent = "AuthAgent/1.0";
        httpContext.User = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity("TestAuth"));

        var userId = Guid.NewGuid();
        using var context = TestContext.Create(userId: userId, httpContext: httpContext);
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var faqItem = await TestDataFactory.SeedFaqItemAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id);

        var handler = new VotesCreateVoteCommandHandler(
            context.DbContext,
            context.SessionService,
            context.HttpContextAccessor);
        var request = new VotesCreateVoteCommand
        {
            Like = true,
            UnLikeReason = null,
            FaqItemId = faqItem.Id
        };

        var id = await handler.Handle(request, CancellationToken.None);
        var vote = await context.DbContext.Votes.FindAsync(id);

        Assert.NotNull(vote);
        Assert.Equal(userId.ToString(), vote!.UserPrint);
    }

    [Fact]
    public async Task CreateVote_ThrowsWhenHttpContextMissing()
    {
        using var context = TestContext.Create(httpContext: null);
        var handler = new VotesCreateVoteCommandHandler(
            context.DbContext,
            context.SessionService,
            context.HttpContextAccessor);
        var request = new VotesCreateVoteCommand
        {
            Like = true,
            UnLikeReason = null,
            FaqItemId = Guid.NewGuid()
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(401, exception.ErrorCode);
    }

    [Fact]
    public async Task UpdateVote_UpdatesExistingVote()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("198.51.100.10");
        httpContext.Request.Headers.UserAgent = "UpdateAgent/2.0";

        using var context = TestContext.Create(httpContext: httpContext);
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var faqItem = await TestDataFactory.SeedFaqItemAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id);
        var vote = await TestDataFactory.SeedVoteAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faqItem.Id,
            like: true);

        var handler = new VotesUpdateVoteCommandHandler(
            context.DbContext,
            context.SessionService,
            context.HttpContextAccessor);
        var request = new VotesUpdateVoteCommand
        {
            Id = vote.Id,
            Like = false,
            UnLikeReason = UnLikeReason.LengthIssue,
            FaqItemId = faqItem.Id
        };

        var identity = VoteRequestContext.GetIdentity(context.SessionService, context.HttpContextAccessor);
        await handler.Handle(request, CancellationToken.None);

        var updated = await context.DbContext.Votes.FindAsync(vote.Id);
        Assert.NotNull(updated);
        Assert.False(updated!.Like);
        Assert.Equal(UnLikeReason.LengthIssue, updated.UnLikeReason);
        Assert.Equal(identity.UserPrint, updated.UserPrint);
        Assert.Equal(identity.Ip, updated.Ip);
        Assert.Equal(identity.UserAgent, updated.UserAgent);
    }

    [Fact]
    public async Task UpdateVote_ThrowsWhenUnLikeReasonMissing()
    {
        using var context = TestContext.Create(httpContext: new DefaultHttpContext());
        var handler = new VotesUpdateVoteCommandHandler(
            context.DbContext,
            context.SessionService,
            context.HttpContextAccessor);
        var request = new VotesUpdateVoteCommand
        {
            Id = Guid.NewGuid(),
            Like = false,
            UnLikeReason = null,
            FaqItemId = Guid.NewGuid()
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(400, exception.ErrorCode);
    }

    [Fact]
    public async Task UpdateVote_ThrowsWhenMissing()
    {
        using var context = TestContext.Create(httpContext: new DefaultHttpContext());
        var handler = new VotesUpdateVoteCommandHandler(
            context.DbContext,
            context.SessionService,
            context.HttpContextAccessor);
        var request = new VotesUpdateVoteCommand
        {
            Id = Guid.NewGuid(),
            Like = true,
            UnLikeReason = null,
            FaqItemId = Guid.NewGuid()
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task GetVote_ReturnsDto()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var faqItem = await TestDataFactory.SeedFaqItemAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id);
        var vote = await TestDataFactory.SeedVoteAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faqItem.Id);

        var handler = new VotesGetVoteQueryHandler(context.DbContext);
        var result = await handler.Handle(new VotesGetVoteQuery { Id = vote.Id }, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(vote.Id, result!.Id);
        Assert.Equal(vote.Like, result.Like);
        Assert.Equal(vote.UserPrint, result.UserPrint);
        Assert.Equal(vote.UnLikeReason, result.UnLikeReason);
        Assert.Equal(vote.FaqItemId, result.FaqItemId);
    }

    [Fact]
    public async Task GetVoteList_ReturnsPagedItems()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var faqItem = await TestDataFactory.SeedFaqItemAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id);
        await TestDataFactory.SeedVoteAsync(context.DbContext, context.SessionService.TenantId, faqItem.Id);
        await TestDataFactory.SeedVoteAsync(context.DbContext, context.SessionService.TenantId, faqItem.Id);

        var handler = new VotesGetVoteListQueryHandler(context.DbContext);
        var request = new VotesGetVoteListQuery
        {
            Request = new VoteGetAllRequestDto { SkipCount = 0, MaxResultCount = 10 }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }
}