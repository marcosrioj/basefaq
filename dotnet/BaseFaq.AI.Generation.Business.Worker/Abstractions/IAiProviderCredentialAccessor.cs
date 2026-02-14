using BaseFaq.AI.Generation.Business.Worker.Models;

namespace BaseFaq.AI.Generation.Business.Worker.Abstractions;

public interface IAiProviderCredentialAccessor
{
    AiProviderCredential GetCurrent();
}