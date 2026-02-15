using BaseFaq.AI.Generation.Business.Generation.Commands.RequestGeneration;
using BaseFaq.AI.Generation.Business.Generation.Extensions;
using BaseFaq.AI.Generation.Business.Generation.Queries.GetGenerationStatus;
using BaseFaq.AI.Generation.Test.IntegrationTests.Helpers;
using BaseFaq.Models.Ai.Dtos.Generation;
using MediatR;
using Xunit;

namespace BaseFaq.AI.Generation.Test.IntegrationTests.Tests.Infrastructure;

public class DependencyRegistrationTests
{
    [Fact]
    public void AddGenerationBusiness_RegistersGenerationStatusHandler()
    {
        var services = TestServiceCollectionFactory.Create();

        services.AddGenerationBusiness();

        var serviceDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IRequestHandler<GenerationGetStatusQuery, GenerationStatusResponse>));

        Assert.NotNull(serviceDescriptor);
    }

    [Fact]
    public void AddGenerationBusiness_RegistersGenerationRequestHandler()
    {
        var services = TestServiceCollectionFactory.Create();

        services.AddGenerationBusiness();

        var serviceDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IRequestHandler<GenerationRequestCommand, Guid>));

        Assert.NotNull(serviceDescriptor);
    }
}