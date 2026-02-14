using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BaseFaq.AI.Common.Providers.Options;

public sealed class AiProviderOptionsValidator(IHostEnvironment hostEnvironment) : IValidateOptions<AiProviderOptions>
{
    public ValidateOptionsResult Validate(string? name, AiProviderOptions options)
    {
        if (!string.IsNullOrWhiteSpace(name) && !string.Equals(name, AiProviderOptions.Name, StringComparison.Ordinal))
        {
            return ValidateOptionsResult.Skip;
        }

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Provider))
        {
            failures.Add("AiProvider:Provider is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Model))
        {
            failures.Add("AiProvider:Model is required.");
        }

        if (!IsAllowedSlot(options.ActiveKeySlot))
        {
            failures.Add("AiProvider:ActiveKeySlot must be either 'Primary' or 'Secondary'.");
        }

        var hasAnyApiKey = !string.IsNullOrWhiteSpace(options.PrimaryApiKey) ||
                           !string.IsNullOrWhiteSpace(options.SecondaryApiKey);

        if (options.RequireApiKey && !hasAnyApiKey)
        {
            failures.Add("AiProvider:RequireApiKey is enabled but no API key is configured.");
        }

        if (!hostEnvironment.IsDevelopment() && !hasAnyApiKey)
        {
            failures.Add("At least one AiProvider API key is required outside development.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static bool IsAllowedSlot(string slot) =>
        string.Equals(slot, AiProviderOptions.PrimarySlot, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(slot, AiProviderOptions.SecondarySlot, StringComparison.OrdinalIgnoreCase);
}