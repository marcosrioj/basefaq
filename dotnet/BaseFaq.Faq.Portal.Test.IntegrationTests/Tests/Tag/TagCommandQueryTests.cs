using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Portal.Business.Tag.Commands.CreateTag;
using BaseFaq.Faq.Portal.Business.Tag.Commands.DeleteTag;
using BaseFaq.Faq.Portal.Business.Tag.Commands.UpdateTag;
using BaseFaq.Faq.Portal.Business.Tag.Queries.GetTag;
using BaseFaq.Faq.Portal.Business.Tag.Queries.GetTagList;
using BaseFaq.Faq.Portal.Test.IntegrationTests.Helpers;
using BaseFaq.Models.Faq.Dtos.Tag;
using Xunit;

namespace BaseFaq.Faq.Portal.Test.IntegrationTests.Tests.Tag;

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
    public async Task DeleteTag_SoftDeletesEntity()
    {
        using var context = TestContext.Create();
        var tag = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "billing");

        var handler = new TagsDeleteTagCommandHandler(context.DbContext);
        await handler.Handle(new TagsDeleteTagCommand { Id = tag.Id }, CancellationToken.None);

        context.DbContext.SoftDeleteFiltersEnabled = false;
        var deleted = await context.DbContext.Tags.FindAsync(tag.Id);
        Assert.NotNull(deleted);
        Assert.True(deleted!.IsDeleted);
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

    [Fact]
    public async Task GetTagList_SortsByExplicitField()
    {
        using var context = TestContext.Create();
        await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "alpha");
        await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "beta");

        var handler = new TagsGetTagListQueryHandler(context.DbContext);
        var request = new TagsGetTagListQuery
        {
            Request = new TagGetAllRequestDto
            {
                SkipCount = 0,
                MaxResultCount = 10,
                Sorting = "value DESC"
            }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal("beta", result.Items[0].Value);
        Assert.Equal("alpha", result.Items[1].Value);
    }

    [Fact]
    public async Task GetTagList_FallsBackToUpdatedDateWhenSortingInvalid()
    {
        using var context = TestContext.Create();
        var first = await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "zulu");
        await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "alpha");
        first.Value = "zulu-updated";
        await context.DbContext.SaveChangesAsync();

        var handler = new TagsGetTagListQueryHandler(context.DbContext);
        var request = new TagsGetTagListQuery
        {
            Request = new TagGetAllRequestDto
            {
                SkipCount = 0,
                MaxResultCount = 10,
                Sorting = "unknown DESC"
            }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal("zulu-updated", result.Items[0].Value);
        Assert.Equal("alpha", result.Items[1].Value);
    }

    [Fact]
    public async Task GetTagList_SortsByMultipleFields()
    {
        using var context = TestContext.Create();

        var tagA = new Common.Persistence.FaqDb.Entities.Tag
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Value = "beta",
            TenantId = context.SessionService.TenantId
        };
        var tagB = new Common.Persistence.FaqDb.Entities.Tag
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Value = "beta",
            TenantId = context.SessionService.TenantId
        };
        var tagC = new Common.Persistence.FaqDb.Entities.Tag
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Value = "alpha",
            TenantId = context.SessionService.TenantId
        };

        context.DbContext.Tags.AddRange(tagA, tagB, tagC);
        await context.DbContext.SaveChangesAsync();

        var handler = new TagsGetTagListQueryHandler(context.DbContext);
        var request = new TagsGetTagListQuery
        {
            Request = new TagGetAllRequestDto
            {
                SkipCount = 0,
                MaxResultCount = 10,
                Sorting = "value DESC, id ASC"
            }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal(tagA.Id, result.Items[0].Id);
        Assert.Equal(tagB.Id, result.Items[1].Id);
        Assert.Equal(tagC.Id, result.Items[2].Id);
    }

    [Fact]
    public async Task GetTagList_AppliesPaginationWindow()
    {
        using var context = TestContext.Create();
        await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "charlie");
        await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "alpha");
        await TestDataFactory.SeedTagAsync(context.DbContext, context.SessionService.TenantId, "bravo");

        var handler = new TagsGetTagListQueryHandler(context.DbContext);
        var request = new TagsGetTagListQuery
        {
            Request = new TagGetAllRequestDto
            {
                SkipCount = 1,
                MaxResultCount = 1,
                Sorting = "value ASC"
            }
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal(3, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("bravo", result.Items[0].Value);
    }
}