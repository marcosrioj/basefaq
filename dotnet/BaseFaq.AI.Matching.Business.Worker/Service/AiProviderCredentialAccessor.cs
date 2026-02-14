using BaseFaq.AI.Matching.Business.Worker.Abstractions;
using BaseFaq.AI.Matching.Business.Worker.Models;
using BaseFaq.AI.Matching.Business.Worker.Options;
using Microsoft.Extensions.Options;

namespace BaseFaq.AI.Matching.Business.Worker.Service;

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