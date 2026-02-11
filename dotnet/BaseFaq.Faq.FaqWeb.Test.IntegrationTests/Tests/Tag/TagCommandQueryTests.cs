using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Business.Tag.Commands.CreateTag;
using BaseFaq.Faq.FaqWeb.Business.Tag.Commands.UpdateTag;
using BaseFaq.Faq.FaqWeb.Business.Tag.Queries.GetTag;
using BaseFaq.Faq.FaqWeb.Business.Tag.Queries.GetTagList;
using BaseFaq.Faq.FaqWeb.Test.IntegrationTests.Helpers;
using BaseFaq.Models.Faq.Dtos.Tag;
using Xunit;

namespace BaseFaq.Faq.FaqWeb.Test.IntegrationTests.Tests.Tag;

public class TagCommandQueryTests
{
    [Fact]
    public async Task CreateTag_PersistsEntityAndReturnsId()
    {
        using var context = TestContext.Create();

        var handler = new TagsCreateTagCommandHandler(context.DbContext, context.SessionService);
        var request = new TagsCreateTagCommand { Value = "support" };

        var id = await handler.Handle(request, CancellationToken.None);

        var tag = await context.DbContext.Tags.FindAsync(id);
        Assert.NotNull(tag);
        Assert.Equal("support", tag!.Value);
        Assert.Equal(context.SessionService.TenantId, tag.TenantId);
    }

    [Fact]
    public async Task UpdateTag_UpdatesExistingTag()
    {
        using var context = TestContext.Create();
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "shipping");

        var handler = new TagsUpdateTagCommandHandler(context.DbContext);
        var request = new TagsUpdateTagCommand { Id = tag.Id, Value = "returns" };

        await handler.Handle(request, CancellationToken.None);

        var updated = await context.DbContext.Tags.FindAsync(tag.Id);
        Assert.NotNull(updated);
        Assert.Equal("returns", updated!.Value);
    }

    [Fact]
    public async Task UpdateTag_ThrowsWhenMissing()
    {
        using var context = TestContext.Create();
        var handler = new TagsUpdateTagCommandHandler(context.DbContext);
        var request = new TagsUpdateTagCommand { Id = Guid.NewGuid(), Value = "lost" };

        var exception =
            await Assert.ThrowsAsync<ApiErrorException>(() => handler.Handle(request, CancellationToken.None));

        Assert.Equal(404, exception.ErrorCode);
    }

    [Fact]
    public async Task GetTag_ReturnsDto()
    {
        using var context = TestContext.Create();
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "faq");

        var handler = new TagsGetTagQueryHandler(context.DbContext);
        var result = await handler.Handle(new TagsGetTagQuery { Id = tag.Id }, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(tag.Id, result!.Id);
        Assert.Equal("faq", result.Value);
        Assert.Equal(tag.TenantId, result.TenantId);
    }

    [Fact]
    public async Task GetTag_ReturnsNullWhenMissing()
    {
        using var context = TestContext.Create();
        var handler = new TagsGetTagQueryHandler(context.DbContext);

        var result = await handler.Handle(new TagsGetTagQuery { Id = Guid.NewGuid() }, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTagList_ReturnsPagedItems()
    {
        using var context = TestContext.Create();
        await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "alpha");
        await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "beta");

        var handler = new TagsGetTagListQueryHandler(context.DbContext);
        var request = new TagsGetTagListQuery
        {
            Request = new TagGetAllRequestDto { SkipCount = 0, MaxResultCount = 10 }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }
}