using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Faq.Infrastructure.ApiErrorHandling.Exception;

public class ApiErrorConfirmationException(
    string message = "Api error",
    int errorCode = 241,
    TranslationCode translationCode = TranslationCode.None) : System.Exception(message)
{
    public int ErrorCode { get; private set; } = errorCode;
    public TranslationCode TranslationCode { get; private set; } = translationCode;
}