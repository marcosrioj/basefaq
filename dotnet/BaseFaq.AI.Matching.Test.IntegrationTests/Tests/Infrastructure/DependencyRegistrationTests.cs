using BaseFaq.AI.Matching.Business.Matching.Abstractions;
using BaseFaq.AI.Matching.Business.Matching.Extensions;
using BaseFaq.AI.Matching.Test.IntegrationTests.Helpers;
using Xunit;

namespace BaseFaq.AI.Matching.Test.IntegrationTests.Tests.Infrastructure;

public class DependencyRegistrationTests
{
    [Fact]
    public void AddMatchingBusiness_RegistersMatchingServices()
    {
        var services = TestServiceCollectionFactory.Create();

        services.AddMatchingBusiness();

        var statusServiceDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IMatchingStatusService));
        var faqItemValidationServiceDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IMatchingFaqItemValidationService));
        var requestServiceDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IMatchingRequestService));
        var requestPublisherDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IMatchingRequestPublisher));

        Assert.NotNull(statusServiceDescriptor);
        Assert.NotNull(faqItemValidationServiceDescriptor);
        Assert.NotNull(requestServiceDescriptor);
        Assert.NotNull(requestPublisherDescriptor);
    }
}