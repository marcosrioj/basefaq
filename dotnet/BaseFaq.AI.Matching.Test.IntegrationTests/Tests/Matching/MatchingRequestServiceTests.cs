using BaseFaq.AI.Matching.Business.Matching.Abstractions;
using BaseFaq.AI.Matching.Business.Matching.Service;
using Xunit;

namespace BaseFaq.AI.Matching.Test.IntegrationTests.Tests.Matching;

public class MatchingRequestServiceTests
{
    [Fact]
    public async Task EnqueueAsync_ThrowsWhenFaqItemIdMissing()
    {
        var service = new MatchingRequestService(new FakeFaqItemValidationService());
        var request = new MatchingRequestDto(
            Guid.NewGuid(),
            Guid.Empty,
            "shipping details",
            "en",
            null,
            null);

        await Assert.ThrowsAsync<ArgumentException>(() => service.EnqueueAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task EnqueueAsync_ThrowsWhenFaqItemValidationFails()
    {
        var service = new MatchingRequestService(new FakeFaqItemValidationService(shouldThrow: true));
        var request = new MatchingRequestDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "shipping details",
            "en",
            null,
            null);

        await Assert.ThrowsAsync<ArgumentException>(() => service.EnqueueAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task EnqueueAsync_ReturnsAcceptedWhenRequestIsValid()
    {
        var service = new MatchingRequestService(new FakeFaqItemValidationService());
        var request = new MatchingRequestDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "shipping details",
            "en",
            "idem-key-1",
            Guid.NewGuid());

        var result = await service.EnqueueAsync(request, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.CorrelationId);
        Assert.True(result.QueuedUtc <= DateTime.UtcNow);
    }

    private sealed class FakeFaqItemValidationService(bool shouldThrow = false) : IMatchingFaqItemValidationService
    {
        public Task EnsureFaqItemExistsAsync(Guid faqItemId, CancellationToken token)
        {
            if (shouldThrow)
            {
                throw new ArgumentException("Invalid FaqItemId.", nameof(faqItemId));
            }

            return Task.CompletedTask;
        }
    }
}