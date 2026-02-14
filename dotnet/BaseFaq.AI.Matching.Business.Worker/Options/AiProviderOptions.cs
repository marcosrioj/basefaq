namespace BaseFaq.AI.Matching.Business.Worker.Options;

public sealed class AiProviderOptions
{
    public const string Name = "AiProvider";
    public const string PrimarySlot = "Primary";
    public const string SecondarySlot = "Secondary";

    public string Provider { get; set; } = "openai";
    public string Model { get; set; } = "text-embedding-3-small";
    public string ActiveKeySlot { get; set; } = PrimarySlot;
    public string? PrimaryApiKey { get; set; }
    public string? SecondaryApiKey { get; set; }
    public bool RequireApiKey { get; set; } = false;
}