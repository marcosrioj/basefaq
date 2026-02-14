using BaseFaq.AI.Common.Providers.Models;

namespace BaseFaq.AI.Common.Providers.Abstractions;

public interface IAiProviderCredentialAccessor
{
    AiProviderCredential GetCurrent();
}