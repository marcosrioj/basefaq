using BaseFaq.AI.Matching.Business.Worker.Models;

namespace BaseFaq.AI.Matching.Business.Worker.Abstractions;

public interface IAiProviderCredentialAccessor
{
    AiProviderCredential GetCurrent();
}