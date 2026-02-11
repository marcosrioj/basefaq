using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaqContentRef;
using BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaqContentRef;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqContentRef;
using BaseFaq.Faq.FaqWeb.Business.Faq.Queries.GetFaqContentRefList;
using BaseFaq.Faq.FaqWeb.Test.IntegrationTests.Helpers;
using BaseFaq.Models.Faq.Dtos.FaqContentRef;
using Xunit;

namespace BaseFaq.Faq.FaqWeb.Test.IntegrationTests.Tests.FaqContentRef;

public class FaqContentRefCommandQueryTests
{
    [Fact]
    public async Task CreateFaqContentRef_PersistsEntityAndReturnsId()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var contentRef = await TestDataFactory.SeedContentRefAsync(context.DbContext, context.SessionService.TenantId);

        var handler = new FaqContentRefsCreateFaqContentRefCommandHandler(
            context.DbContext,
            context.SessionService);
        var request = new FaqContentRefsCreateFaqContentRefCommand
        {
            FaqId = faq.Id,
            ContentRefId = contentRef.Id
        };

        var id = await handler.Handle(request, CancellationToken.None);

        var faqContentRef = await context.DbContext.FaqContentRefs.FindAsync(id);
        Assert.NotNull(faqContentRef);
        Assert.Equal(faq.Id, faqContentRef!.FaqId);
        Assert.Equal(contentRef.Id, faqContentRef.ContentRefId);
        Assert.Equal(context.SessionService.TenantId, faqContentRef.TenantId);
    }

    [Fact]
    public async Task CreateFaqContentRef_ThrowsWhenContentRefMissing()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);

        var handler = new FaqContentRefsCreateFaqContentRefCommandHandler(
            context.DbContext,
            context.SessionService);
        var request = new FaqContentRefsCreateFaqContentRefCommand
        {
            FaqId = faq.Id,
            ContentRefId = Guid.NewGuid()
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task CreateFaqContentRef_ThrowsWhenFaqMissing()
    {
        using var context = TestContext.Create();
        var contentRef = await TestDataFactory.SeedContentRefAsync(context.DbContext, context.SessionService.TenantId);

        var handler = new FaqContentRefsCreateFaqContentRefCommandHandler(
            context.DbContext,
            context.SessionService);
        var request = new FaqContentRefsCreateFaqContentRefCommand
        {
            FaqId = Guid.NewGuid(),
            ContentRefId = contentRef.Id
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task UpdateFaqContentRef_UpdatesExistingFaqContentRef()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var otherFaq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId, "Other");
        var contentRef = await TestDataFactory.SeedContentRefAsync(context.DbContext, context.SessionService.TenantId);
        var otherContentRef = await TestDataFactory.SeedContentRefAsync(
            context.DbContext,
            context.SessionService.TenantId,
            locator: "other");
        var faqContentRef = await TestDataFactory.SeedFaqContentRefAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id,
            contentRef.Id);

        var handler = new FaqContentRefsUpdateFaqContentRefCommandHandler(context.DbContext);
        var request = new FaqContentRefsUpdateFaqContentRefCommand
        {
            Id = faqContentRef.Id,
            FaqId = otherFaq.Id,
            ContentRefId = otherContentRef.Id
        };

        await handler.Handle(request, CancellationToken.None);

        var updated = await context.DbContext.FaqContentRefs.FindAsync(faqContentRef.Id);
        Assert.NotNull(updated);
        Assert.Equal(otherFaq.Id, updated!.FaqId);
        Assert.Equal(otherContentRef.Id, updated.ContentRefId);
    }

    [Fact]
    public async Task UpdateFaqContentRef_ThrowsWhenMissing()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var contentRef = await TestDataFactory.SeedContentRefAsync(context.DbContext, context.SessionService.TenantId);

        var handler = new FaqContentRefsUpdateFaqContentRefCommandHandler(context.DbContext);
        var request = new FaqContentRefsUpdateFaqContentRefCommand
        {
            Id = Guid.NewGuid(),
            FaqId = faq.Id,
            ContentRefId = contentRef.Id
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task UpdateFaqContentRef_ThrowsWhenFaqMissing()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var contentRef = await TestDataFactory.SeedContentRefAsync(context.DbContext, context.SessionService.TenantId);
        var faqContentRef = await TestDataFactory.SeedFaqContentRefAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id,
            contentRef.Id);

        var handler = new FaqContentRefsUpdateFaqContentRefCommandHandler(context.DbContext);
        var request = new FaqContentRefsUpdateFaqContentRefCommand
        {
            Id = faqContentRef.Id,
            FaqId = Guid.NewGuid(),
            ContentRefId = contentRef.Id
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task UpdateFaqContentRef_ThrowsWhenContentRefMissing()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var contentRef = await TestDataFactory.SeedContentRefAsync(context.DbContext, context.SessionService.TenantId);
        var faqContentRef = await TestDataFactory.SeedFaqContentRefAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id,
            contentRef.Id);

        var handler = new FaqContentRefsUpdateFaqContentRefCommandHandler(context.DbContext);
        var request = new FaqContentRefsUpdateFaqContentRefCommand
        {
            Id = faqContentRef.Id,
            FaqId = faq.Id,
            ContentRefId = Guid.NewGuid()
        };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task GetFaqContentRef_ReturnsDto()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var contentRef = await TestDataFactory.SeedContentRefAsync(context.DbContext, context.SessionService.TenantId);
        var faqContentRef = await TestDataFactory.SeedFaqContentRefAsync(
            context.DbContext,
            context.SessionService.TenantId,
            faq.Id,
            contentRef.Id);

        var handler = new FaqContentRefsGetFaqContentRefQueryHandler(context.DbContext);
        var result = await handler.Handle(
            new FaqContentRefsGetFaqContentRefQuery { Id = faqContentRef.Id },
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(faqContentRef.Id, result!.Id);
        Assert.Equal(faq.Id, result.FaqId);
        Assert.Equal(contentRef.Id, result.ContentRefId);
    }

    [Fact]
    public async Task GetFaqContentRefList_ReturnsPagedItems()
    {
        using var context = TestContext.Create();
        var faq = await TestDataFactory.SeedFaqAsync(context.DbContext, context.SessionService.TenantId);
        var contentRef1 =
            await TestDataFactory.SeedContentRefAsync(context.DbContext, context.SessionService.TenantId,
                locator: "one");
        var contentRef2 =
            await TestDataFactory.SeedContentRefAsync(context.DbContext, context.SessionService.TenantId,
                locator: "two");
        await TestDataFactory.SeedFaqContentRefAsync(context.DbContext, context.SessionService.TenantId, faq.Id,
            contentRef1.Id);
        await TestDataFactory.SeedFaqContentRefAsync(context.DbContext, context.SessionService.TenantId, faq.Id,
            contentRef2.Id);

        var handler = new FaqContentRefsGetFaqContentRefListQueryHandler(context.DbContext);
        var request = new FaqContentRefsGetFaqContentRefListQuery
        {
            Request = new FaqContentRefGetAllRequestDto { SkipCount = 0, MaxResultCount = 10 }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }
}