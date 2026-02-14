namespace BaseFaq.AI.Common.Providers.Models;

public sealed record AiProviderCredential(
    string Provider,
    string Model,
    string? ApiKey,
    string SelectedSlot);