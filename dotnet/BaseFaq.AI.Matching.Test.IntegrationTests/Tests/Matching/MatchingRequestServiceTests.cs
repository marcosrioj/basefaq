using BaseFaq.AI.Matching.Business.Matching.Abstractions;
using BaseFaq.AI.Matching.Business.Matching.Commands.RequestMatching;
using BaseFaq.Models.Ai.Dtos.Matching;
using Xunit;

namespace BaseFaq.AI.Matching.Test.IntegrationTests.Tests.Matching;

public class MatchingRequestCommandHandlerTests
{
    [Fact]
    public async Task Handle_ThrowsWhenFaqItemIdMissing()
    {
        var handler = new MatchingRequestCommandHandler(
            new FakeFaqItemValidationService(),
            new FakeMatchingRequestPublisher());
        var command = new MatchingRequestCommand(new MatchingRequestDto(
            Guid.NewGuid(),
            Guid.Empty,
            "shipping details",
            "en",
            null,
            null), "idem-header-1");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsWhenFaqItemValidationFails()
    {
        var handler = new MatchingRequestCommandHandler(
            new FakeFaqItemValidationService(shouldThrow: true),
            new FakeMatchingRequestPublisher());
        var command = new MatchingRequestCommand(new MatchingRequestDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "shipping details",
            "en",
            null,
            null), "idem-header-2");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsWhenIdempotencyHeaderMissing()
    {
        var handler = new MatchingRequestCommandHandler(
            new FakeFaqItemValidationService(),
            new FakeMatchingRequestPublisher());
        var command = new MatchingRequestCommand(new MatchingRequestDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "shipping details",
            "en",
            null,
            null), null);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReturnsAcceptedWhenRequestIsValid()
    {
        var handler = new MatchingRequestCommandHandler(
            new FakeFaqItemValidationService(),
            new FakeMatchingRequestPublisher());
        var command = new MatchingRequestCommand(new MatchingRequestDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "shipping details",
            "en",
            "idem-key-1",
            Guid.NewGuid()), "idem-key-1");

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.NotEqual(Guid.Empty, result);
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

    private sealed class FakeMatchingRequestPublisher : IMatchingRequestPublisher
    {
        public Task PublishAsync(
            MatchingRequestDto request,
            string idempotencyKey,
            Guid correlationId,
            DateTime queuedUtc,
            CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}