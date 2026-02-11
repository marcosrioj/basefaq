using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaqTag;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaqTag;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqTag;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqTagList;
using BaseFaq.Faq.FaqWeb.Test.IntegrationTests.Helpers;
using BaseFaq.Models.Faq.Dtos.FaqTag;
using Xunit;

namespace BaseFaq.Faq.FaqWeb.Test.IntegrationTests.Tests.FaqTag;

public class FaqTagCommandQueryTests
{
    [Fact]
    public async Task CreateFaqTag_PersistsEntityAndReturnsId()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId);

        var handler = new FaqTagsCreateFaqTagCommandHandler(context.DbContext, context.SessionService);
        var request = new FaqTagsCreateFaqTagCommand { FaqId = faq.Id, TagId = tag.Id };

        var id = await handler.Handle(request, CancellationToken.None);

        var faqTag = await context.DbContext.FaqTags.FindAsync(id);
        Assert.NotNull(faqTag);
        Assert.Equal(faq.Id, faqTag!.FaqId);
        Assert.Equal(tag.Id, faqTag.TagId);
        Assert.Equal(context.SessionService.TenantId, faqTag.TenantId);
    }

    [Fact]
    public async Task CreateFaqTag_ThrowsWhenFaqMissing()
    {
        using var context = TestContext.Create();
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId);

        var handler = new FaqTagsCreateFaqTagCommandHandler(context.DbContext, context.SessionService);
        var request = new FaqTagsCreateFaqTagCommand { FaqId = Guid.NewGuid(), TagId = tag.Id };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task CreateFaqTag_ThrowsWhenTagMissing()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);

        var handler = new FaqTagsCreateFaqTagCommandHandler(context.DbContext, context.SessionService);
        var request = new FaqTagsCreateFaqTagCommand { FaqId = faq.Id, TagId = Guid.NewGuid() };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task UpdateFaqTag_UpdatesExistingFaqTag()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "first");
        var otherTag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "second");
        var faqTag = await TestDataFactory.SeedFaqTagAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id,
            tag.Id);

        var handler = new FaqTagsUpdateFaqTagCommandHandler(context.DbContext);
        var request = new FaqTagsUpdateFaqTagCommand
        {
            Id = faqTag.Id,
            FaqId = faq.Id,
            TagId = otherTag.Id
        };

        await handler.Handle(request, CancellationToken.None);

        var updated = await context.DbContext.FaqTags.FindAsync(faqTag.Id);
        Assert.NotNull(updated);
        Assert.Equal(otherTag.Id, updated!.TagId);
    }

    [Fact]
    public async Task UpdateFaqTag_ThrowsWhenMissing()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId);

        var handler = new FaqTagsUpdateFaqTagCommandHandler(context.DbContext);
        var request = new FaqTagsUpdateFaqTagCommand
        {
            Id = Guid.NewGuid(),
            FaqId = faq.Id,
            TagId = tag.Id
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task UpdateFaqTag_ThrowsWhenFaqMissing()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId);
        var faqTag = await TestDataFactory.SeedFaqTagAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id,
            tag.Id);

        var handler = new FaqTagsUpdateFaqTagCommandHandler(context.DbContext);
        var request = new FaqTagsUpdateFaqTagCommand
        {
            Id = faqTag.Id,
            FaqId = Guid.NewGuid(),
            TagId = tag.Id
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task UpdateFaqTag_ThrowsWhenTagMissing()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId);
        var faqTag = await TestDataFactory.SeedFaqTagAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id,
            tag.Id);

        var handler = new FaqTagsUpdateFaqTagCommandHandler(context.DbContext);
        var request = new FaqTagsUpdateFaqTagCommand
        {
            Id = faqTag.Id,
            FaqId = faq.Id,
            TagId = Guid.NewGuid()
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task GetFaqTag_ReturnsDto()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId);
        var faqTag = await TestDataFactory.SeedFaqTagAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id,
            tag.Id);

        var handler = new FaqTagsGetFaqTagQueryHandler(context.DbContext);
        var result = await handler.Handle(new FaqTagsGetFaqTagQuery { Id = faqTag.Id }, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(faqTag.Id, result!.Id);
        Assert.Equal(faq.Id, result.FaqId);
        Assert.Equal(tag.Id, result.TagId);
    }

    [Fact]
    public async Task GetFaqTagList_ReturnsPagedItems()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var tag1 = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "one");
        var tag2 = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "two");
        await TestDataFactory.SeedFaqTagAsync(context.DbContext, context.SessionService.TenantId, faq.Id, tag1.Id);
        await TestDataFactory.SeedFaqTagAsync(context.DbContext, context.SessionService.TenantId, faq.Id, tag2.Id);

        var handler = new FaqTagsGetFaqTagListQueryHandler(context.DbContext);
        var request = new FaqTagsGetFaqTagListQuery
        {
            Request = new FaqTagGetAllRequestDto { SkipCount = 0, MaxResultCount = 10 }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }
}