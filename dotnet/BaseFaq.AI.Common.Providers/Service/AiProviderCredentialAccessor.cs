using BaseFaq.AI.Common.Providers.Abstractions;
using BaseFaq.AI.Common.Providers.Models;
using BaseFaq.AI.Common.Providers.Options;
using Microsoft.Extensions.Options;

namespace BaseFaq.AI.Common.Providers.Service;

public sealed class AiProviderCredentialAccessor(IOptionsMonitor<AiProviderOptions> optionsMonitor)
    : IAiProviderCredentialAccessor
{
    public AiProviderCredential GetCurrent()
    {
        var options = optionsMonitor.CurrentValue;
        var useSecondary = string.Equals(options.ActiveKeySlot, AiProviderOptions.SecondarySlot,
            StringComparison.OrdinalIgnoreCase);

        var preferredKey = useSecondary ? options.SecondaryApiKey : options.PrimaryApiKey;
        var fallbackKey = useSecondary ? options.PrimaryApiKey : options.SecondaryApiKey;

        var selectedSlot = !string.IsNullOrWhiteSpace(preferredKey)
            ? (useSecondary ? AiProviderOptions.SecondarySlot : AiProviderOptions.PrimarySlot)
            : (!string.IsNullOrWhiteSpace(fallbackKey)
                ? (useSecondary ? AiProviderOptions.PrimarySlot : AiProviderOptions.SecondarySlot)
                : "None");

        var apiKey = !string.IsNullOrWhiteSpace(preferredKey) ? preferredKey : fallbackKey;

        return new AiProviderCredential(
            options.Provider,
            options.Model,
            apiKey,
            selectedSlot);
    }
}