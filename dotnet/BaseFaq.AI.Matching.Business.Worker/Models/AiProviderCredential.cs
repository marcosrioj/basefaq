namespace BaseFaq.AI.Matching.Business.Worker.Models;

public sealed record AiProviderCredential(
    string Provider,
    string Model,
    string? ApiKey,
    string SelectedSlot);