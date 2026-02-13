namespace BaseFaq.AI.Common.Contracts.Generation;

public static class GenerationEventNames
{
    public const string ReadyExchange = "faq.generation.ready.v1";
    public const string FailedExchange = "faq.generation.failed.v1";
    public const string ExchangeType = "fanout";
}