using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Faq.Infrastructure.ApiErrorHandling.Exception;

public class ApiErrorException(
    string message = "Api error",
    int errorCode = 240,
    TranslationCode translationCode = TranslationCode.None,
    object? dataObject = null) : System.Exception(message)
{
    public int ErrorCode { get; private set; } = errorCode;
    public TranslationCode TranslationCode { get; private set; } = translationCode;
    public object? DataObject { get; } = dataObject;
}