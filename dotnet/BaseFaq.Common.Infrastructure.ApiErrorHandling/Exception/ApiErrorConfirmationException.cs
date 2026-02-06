using BaseFaq.Models.Common.Enums;

namespace BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;

public class ApiErrorConfirmationException(
    string message = "Api error",
    int errorCode = 241) : System.Exception(message)
{
    public int ErrorCode { get; private set; } = errorCode;
}