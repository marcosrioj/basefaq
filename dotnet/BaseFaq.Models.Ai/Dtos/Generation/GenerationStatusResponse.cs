namespace BaseFaq.Models.Ai.Dtos.Generation;

public sealed record GenerationStatusResponse(string Service, string Status, DateTime UtcTimestamp);