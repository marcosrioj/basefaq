using BaseFaq.AI.Generation.Business.Worker.Service;
using Xunit;

namespace BaseFaq.AI.Generation.Test.IntegrationTests.Tests.Infrastructure;

public class GenerationFaqWriteIdempotencyTests
{
    [Fact]
    public void CreateDeterministicFaqItemId_ReturnsSameId_ForSameInput()
    {
        var correlationId = Guid.NewGuid();
        var faqId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var first = GenerationFaqWriteIdempotencyHelper.CreateDeterministicFaqItemId(correlationId, faqId, tenantId);
        var second = GenerationFaqWriteIdempotencyHelper.CreateDeterministicFaqItemId(correlationId, faqId, tenantId);

        Assert.Equal(first, second);
    }

    [Fact]
    public void CreateDeterministicFaqItemId_ReturnsDifferentId_ForDifferentInput()
    {
        var correlationId = Guid.NewGuid();
        var faqId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var first = GenerationFaqWriteIdempotencyHelper.CreateDeterministicFaqItemId(correlationId, faqId, tenantId);
        var second = GenerationFaqWriteIdempotencyHelper.CreateDeterministicFaqItemId(Guid.NewGuid(), faqId, tenantId);

        Assert.NotEqual(first, second);
    }
}