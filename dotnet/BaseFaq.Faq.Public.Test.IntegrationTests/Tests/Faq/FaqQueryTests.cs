using BaseFaq.Faq.Public.Business.Faq.Queries.GetFaq;
using BaseFaq.Faq.Public.Business.Faq.Queries.GetFaqList;
using BaseFaq.Faq.Public.Test.IntegrationTests.Helpers;
using BaseFaq.Models.Faq.Dtos.Faq;
using Xunit;

namespace BaseFaq.Faq.Public.Test.IntegrationTests.Tests.Faq;

public class FaqQueryTests
{
    [Fact]
    public async Task GetFaqList_FiltersByFaqIdsAndIncludesRelations()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId, "Public FAQ");
        var otherFaq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId, "Other");
        var contentRef = await TestDataFactory.SeedContentRefAsync(context.DbContext, context.SessionService.TenantId);
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId);
        await TestDataFactory.SeedFaqItemAsync(context.DbContext, context.SessionService.TenantId, faq.Id);
        await TestDataFactory.SeedFaqContentRefAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id,
            contentRef.Id);
        await TestDataFactory.SeedFaqTagAsync(context.DbContext, context.SessionService.TenantId, faq.Id, tag.Id);
        await TestDataFactory.SeedFaqItemAsync(context.DbContext, context.SessionService.TenantId, otherFaq.Id);

        var handler = new FaqsGetFaqListQueryHandler(context.DbContext);
        var request = new FaqsGetFaqListQuery
        {
            Request = new FaqGetAllRequestDto
            {
                SkipCount = 0,
                MaxResultCount = 10,
                FaqIds = [faq.Id],
                IncludeFaqItems = true,
                IncludeContentRefs = true,
                IncludeTags = true
            }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
        var dto = result.Items.Single();
        Assert.Equal(faq.Id, dto.Id);
        Assert.NotNull(dto.Items);
        Assert.Single(dto.Items!);
        Assert.NotNull(dto.ContentRefs);
        Assert.Single(dto.ContentRefs!);
        Assert.NotNull(dto.Tags);
        Assert.Single(dto.Tags!);
    }

    [Fact]
    public async Task GetFaqById_RespectsIncludeFlags()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var contentRef = await TestDataFactory.SeedContentRefAsync(context.DbContext, context.SessionService.TenantId);
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId);
        await TestDataFactory.SeedFaqItemAsync(context.DbContext, context.SessionService.TenantId, faq.Id);
        await TestDataFactory.SeedFaqContentRefAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id,
            contentRef.Id);
        await TestDataFactory.SeedFaqTagAsync(context.DbContext, context.SessionService.TenantId, faq.Id, tag.Id);

        var handler = new FaqsGetFaqQueryHandler(context.DbContext);
        var request = new FaqsGetFaqQuery
        {
            Id = faq.Id,
            Request = new FaqGetRequestDto
            {
                IncludeFaqItems = true,
                IncludeContentRefs = false,
                IncludeTags = true
            }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result!.Items);
        Assert.Single(result.Items!);
        Assert.Null(result.ContentRefs);
        Assert.NotNull(result.Tags);
        Assert.Single(result.Tags!);
    }
}