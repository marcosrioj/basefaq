using BaseFaq.AI.Generation.Business.Worker.Options;
using BaseFaq.AI.Generation.Business.Worker.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace BaseFaq.AI.Generation.Test.IntegrationTests.Tests.Infrastructure;

public sealed class AiProviderSecretConfigurationTests
{
    [Fact]
    public void CredentialAccessor_UsesFallbackSlot_WhenActiveSlotIsMissing()
    {
        var options = Options.Create(new AiProviderOptions
        {
            Provider = "openai",
            Model = "gpt-4o-mini",
            ActiveKeySlot = AiProviderOptions.PrimarySlot,
            PrimaryApiKey = null,
            SecondaryApiKey = "secondary-key"
        });
        var accessor = new AiProviderCredentialAccessor(new StaticOptionsMonitor<AiProviderOptions>(options.Value));

        var credential = accessor.GetCurrent();

        Assert.Equal("openai", credential.Provider);
        Assert.Equal("gpt-4o-mini", credential.Model);
        Assert.Equal("secondary-key", credential.ApiKey);
        Assert.Equal(AiProviderOptions.SecondarySlot, credential.SelectedSlot);
    }

    [Fact]
    public void OptionsValidator_FailsOutsideDevelopment_WhenNoApiKeyConfigured()
    {
        var validator = new AiProviderOptionsValidator(new TestHostEnvironment(Environments.Production));
        var options = new AiProviderOptions
        {
            Provider = "openai",
            Model = "gpt-4o-mini",
            ActiveKeySlot = AiProviderOptions.PrimarySlot,
            PrimaryApiKey = null,
            SecondaryApiKey = null
        };

        var result = validator.Validate(AiProviderOptions.Name, options);

        Assert.True(result.Failed);
    }

    private sealed class StaticOptionsMonitor<T>(T value) : IOptionsMonitor<T>
    {
        public T CurrentValue => value;

        public T Get(string? name) => value;

        public IDisposable? OnChange(Action<T, string?> listener) => null;
    }

    private sealed class TestHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = nameof(TestHostEnvironment);
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();

        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
            new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}