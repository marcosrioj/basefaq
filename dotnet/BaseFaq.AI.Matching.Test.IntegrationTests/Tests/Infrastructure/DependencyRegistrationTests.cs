using BaseFaq.AI.Matching.Business.Matching.Abstractions;
using BaseFaq.AI.Matching.Business.Matching.Commands.RequestMatching;
using BaseFaq.AI.Matching.Business.Matching.Extensions;
using BaseFaq.AI.Matching.Business.Matching.Queries.GetMatchingStatus;
using BaseFaq.AI.Matching.Test.IntegrationTests.Helpers;
using BaseFaq.Models.Ai.Dtos.Matching;
using MediatR;
using Xunit;

namespace BaseFaq.AI.Matching.Test.IntegrationTests.Tests.Infrastructure;

public class DependencyRegistrationTests
{
    [Fact]
    public void AddMatchingBusiness_RegistersMatchingServices()
    {
        var services = TestServiceCollectionFactory.Create();

        services.AddMatchingBusiness();

        var faqItemValidationServiceDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IMatchingFaqItemValidationService));
        var requestPublisherDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IMatchingRequestPublisher));
        var statusHandlerDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IRequestHandler<MatchingGetStatusQuery, MatchingStatusResponse>));
        var requestHandlerDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(IRequestHandler<MatchingRequestCommand, MatchingRequestAcceptedResponse>));

        Assert.NotNull(faqItemValidationServiceDescriptor);
        Assert.NotNull(requestPublisherDescriptor);
        Assert.NotNull(statusHandlerDescriptor);
        Assert.NotNull(requestHandlerDescriptor);
    }
}