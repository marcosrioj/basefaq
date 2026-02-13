using Microsoft.Extensions.DependencyInjection;

namespace BaseFaq.AI.Generation.Test.IntegrationTests.Helpers;

public static class TestServiceCollectionFactory
{
    public static IServiceCollection Create() => new ServiceCollection();
}