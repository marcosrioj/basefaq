using System.Security.Cryptography;
using System.Text;

namespace BaseFaq.AI.Generation.Business.Worker.Service;

internal static class GenerationFaqWriteIdempotencyHelper
{
    public static Guid CreateDeterministicFaqItemId(Guid correlationId, Guid faqId, Guid tenantId)
    {
        var input = $"{correlationId:N}:{faqId:N}:{tenantId:N}";
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(input));

        hash[6] = (byte)((hash[6] & 0x0F) | (3 << 4));
        hash[8] = (byte)((hash[8] & 0x3F) | 0x80);

        return new Guid(hash);
    }
}