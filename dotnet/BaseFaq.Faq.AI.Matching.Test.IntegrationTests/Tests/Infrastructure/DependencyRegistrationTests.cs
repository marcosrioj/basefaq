using BaseFaq.Faq.AI.Matching.Business.Matching.Abstractions;
using BaseFaq.Faq.AI.Matching.Business.Matching.Extensions;
using BaseFaq.Faq.AI.Matching.Test.IntegrationTests.Helpers;
using Xunit;

namespace BaseFaq.Faq.AI.Matching.Test.IntegrationTests.Tests.Infrastructure;

public class DependencyRegistrationTests
{
    [Fact]
    public void AddMatchingBusiness_RegistersMatchingStatusService()
    {
        var services = TestServiceCollectionFactory.Create();

        services.AddMatchingBusiness();

        var serviceDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IMatchingStatusService));

        Assert.NotNull(serviceDescriptor);
    }
}