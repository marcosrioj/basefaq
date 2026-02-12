using BaseFaq.Faq.Public.Business.FaqItem.Commands.CreateFaqItem;
using BaseFaq.Faq.Public.Business.FaqItem.Queries.SearchFaqItem;
using BaseFaq.Faq.Public.Test.IntegrationTests.Helpers;
using BaseFaq.Models.Faq.Dtos.FaqItem;
using BaseFaq.Models.Faq.Enums;
using Xunit;

namespace BaseFaq.Faq.Public.Test.IntegrationTests.Tests.FaqItem;

public class FaqItemCommandQueryTests
{
    [Fact]
    public async Task CreateFaqItem_PersistsEntityAndReturnsId()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);

        var handler = new FaqItemsCreateFaqItemCommandHandler(context.DbContext, context.SessionService);
        var request = new FaqItemsCreateFaqItemCommand
        {
            Question = "How to sign in?",
            ShortAnswer = "Use your email.",
            Answer = "Sign in with email and password.",
            AdditionalInfo = "Contact support if needed.",
            CtaTitle = "Sign in",
            CtaUrl = "https://example.test/login",
            Sort = 2,
            VoteScore = 5,
            AiConfidenceScore = 70,
            IsActive = true,
            FaqId = faq.Id,
            ContentRefId = null
        };

        var id = await handler.Handle(request, CancellationToken.None);

        var faqItem = await context.DbContext.FaqItems.FindAsync(id);
        Assert.NotNull(faqItem);
        Assert.Equal("How to sign in?", faqItem!.Question);
        Assert.Equal(faq.Id, faqItem.FaqId);
        Assert.Equal(context.SessionService.TenantId, faqItem.TenantId);
    }

    [Fact]
    public async Task SearchFaqItems_OrdersByFaqSortStrategy()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(
            context.DbContext,
            context.SessionService.TenantId,
            sortStrategy: FaqSortStrategy.Vote);

        var lowVote = new Common.Persistence.FaqDb.Entities.FaqItem
        {
            Question = "Low vote",
            ShortAnswer = "Low",
            Answer = "Low",
            AdditionalInfo = "Low",
            CtaTitle = "Low",
            CtaUrl = "https://example.test/low",
            Sort = 1,
            VoteScore = 1,
            AiConfidenceScore = 10,
            IsActive = true,
            FaqId = faq.Id,
            TenantId = context.SessionService.TenantId
        };
        var highVote = new Common.Persistence.FaqDb.Entities.FaqItem
        {
            Question = "High vote",
            ShortAnswer = "High",
            Answer = "High",
            AdditionalInfo = "High",
            CtaTitle = "High",
            CtaUrl = "https://example.test/high",
            Sort = 2,
            VoteScore = 20,
            AiConfidenceScore = 50,
            IsActive = true,
            FaqId = faq.Id,
            TenantId = context.SessionService.TenantId
        };

        context.DbContext.FaqItems.AddRange(lowVote, highVote);
        await context.DbContext.SaveChangesAsync();

        var handler = new FaqItemsSearchFaqItemQueryHandler(context.DbContext);
        var request = new FaqItemsSearchFaqItemQuery
        {
            Request = new FaqItemSearchRequestDto
            {
                SkipCount = 0,
                MaxResultCount = 10,
                FaqIds = [faq.Id]
            }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(highVote.Id, result.Items[0].Id);
        Assert.Equal(lowVote.Id, result.Items[1].Id);
    }

    [Fact]
    public async Task SearchFaqItems_FiltersByFaqIds()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var otherFaq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId, "Other");
        await TestDataFactory.SeedFaqItemAsync(context.DbContext, context.SessionService.TenantId, faq.Id);
        await TestDataFactory.SeedFaqItemAsync(context.DbContext, context.SessionService.TenantId, otherFaq.Id);

        var handler = new FaqItemsSearchFaqItemQueryHandler(context.DbContext);
        var request = new FaqItemsSearchFaqItemQuery
        {
            Request = new FaqItemSearchRequestDto
            {
                SkipCount = 0,
                MaxResultCount = 10,
                FaqIds = [faq.Id]
            }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal(faq.Id, result.Items[0].FaqId);
    }
}