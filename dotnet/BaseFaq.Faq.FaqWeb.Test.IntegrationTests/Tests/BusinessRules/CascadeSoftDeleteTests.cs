using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqContentRef;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqTag;
using BaseFaq.Faq.FaqWeb.Business.FaqItem.Queries.GetFaqItem;
using BaseFaq.Faq.FaqWeb.Test.IntegrationTests.Helpers;
using Xunit;

namespace BaseFaq.Faq.FaqWeb.Test.IntegrationTests.Tests.BusinessRules;

public class CascadeSoftDeleteTests
{
    [Fact]
    public async Task DeletingFaq_SoftDeletesRelatedEntities()
    {
        using var context = TestContext.Create();
        var tenantId = context.SessionService.TenantId;

        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, tenantId);
        var contentRef = await TestDataFactory.SeedContentRefAsync(context.DbContext, tenantId);
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, tenantId);
        var faqItem = await TestDataFactory.SeedFaqItemAsync(context.DbContext, tenantId, faq.Id, contentRef.Id);
        var faqTag = await TestDataFactory.SeedFaqTagAsync(context.DbContext, tenantId, faq.Id, tag.Id);
        var faqContentRef = await TestDataFactory.SeedFaqContentRefAsync(
            context.DbContext,
            tenantId,
            faq.Id,
            contentRef.Id);

        context.DbContext.Faqs.Remove(faq);
        await context.DbContext.SaveChangesAsync();

        var faqItemHandler = new FaqItemsGetFaqItemQueryHandler(context.DbContext);
        var faqTagHandler = new FaqTagsGetFaqTagQueryHandler(context.DbContext);
        var faqContentRefHandler = new FaqContentRefsGetFaqContentRefQueryHandler(context.DbContext);

        Assert.Null(
            await faqItemHandler.Handle(new FaqItemsGetFaqItemQuery { Id = faqItem.Id }, CancellationToken.None));
        Assert.Null(await faqTagHandler.Handle(new FaqTagsGetFaqTagQuery { Id = faqTag.Id }, CancellationToken.None));
        Assert.Null(await faqContentRefHandler.Handle(
            new FaqContentRefsGetFaqContentRefQuery { Id = faqContentRef.Id },
            CancellationToken.None));

        context.DbContext.SoftDeleteFiltersEnabled = false;

        var deletedFaqItem = await context.DbContext.FaqItems.FindAsync(faqItem.Id);
        var deletedFaqTag = await context.DbContext.FaqTags.FindAsync(faqTag.Id);
        var deletedFaqContentRef = await context.DbContext.FaqContentRefs.FindAsync(faqContentRef.Id);

        Assert.NotNull(deletedFaqItem);
        Assert.True(deletedFaqItem!.IsDeleted);
        Assert.NotNull(deletedFaqTag);
        Assert.True(deletedFaqTag!.IsDeleted);
        Assert.NotNull(deletedFaqContentRef);
        Assert.True(deletedFaqContentRef!.IsDeleted);
    }
}