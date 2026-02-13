using BaseFaq.AI.Generation.Business.Generation.Abstractions;
using BaseFaq.AI.Generation.Business.Generation.Extensions;
using BaseFaq.AI.Generation.Test.IntegrationTests.Helpers;
using Xunit;

namespace BaseFaq.AI.Generation.Test.IntegrationTests.Tests.Infrastructure;

public class DependencyRegistrationTests
{
    [Fact]
    public void AddGenerationBusiness_RegistersGenerationStatusService()
    {
        var services = TestServiceCollectionFactory.Create();

        services.AddGenerationBusiness();

        var serviceDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IGenerationStatusService));

        Assert.NotNull(serviceDescriptor);
    }
}