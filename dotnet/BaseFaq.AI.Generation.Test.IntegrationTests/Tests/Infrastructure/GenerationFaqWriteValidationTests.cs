using BaseFaq.AI.Generation.Business.Worker.Abstractions;
using BaseFaq.AI.Generation.Business.Worker.Service;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace BaseFaq.AI.Generation.Test.IntegrationTests.Tests.Infrastructure;

public class GenerationFaqWriteValidationTests
{
    private readonly GenerationFaqWriteService _service = new(
        new StubFaqIntegrationDbContextFactory(),
        NullLogger<GenerationFaqWriteService>.Instance);

    [Fact]
    public async Task WriteAsync_Throws_WhenTenantIdIsEmpty()
    {
        var request = BuildRequest() with { TenantId = Guid.Empty };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.WriteAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task WriteAsync_Throws_WhenQuestionTooLong()
    {
        var request = BuildRequest() with { Question = new string('q', 1001) };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.WriteAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task WriteAsync_Throws_WhenConfidenceScoreOutOfRange()
    {
        var request = BuildRequest() with { AiConfidenceScore = 101 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.WriteAsync(request, CancellationToken.None));
    }

    private static GenerationFaqWriteRequest BuildRequest()
    {
        return new GenerationFaqWriteRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Question",
            "Short answer",
            "Full answer",
            null,
            null,
            null,
            80);
    }

    private sealed class StubFaqIntegrationDbContextFactory : IFaqIntegrationDbContextFactory
    {
        public FaqDbContext Create(Guid tenantId)
        {
            throw new InvalidOperationException("Factory should not be used for validation-only tests.");
        }
    }
}