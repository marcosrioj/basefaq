using System.Text.Json;
using BaseFaq.Faq.Infrastructure.ApiErrorHandling.Exception;
using Microsoft.AspNetCore.Http;

namespace BaseFaq.Faq.Infrastructure.ApiErrorHandling.Middleware;

public class ApiErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiErrorException apiError)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = apiError.ErrorCode;
            context.Response.Headers.Append("Translation-Code", apiError.TranslationCode.ToString());

            if (apiError.DataObject != null)
            {
                await context.Response.WriteAsync(JsonSerializer.Serialize(apiError.DataObject));
            }
            else
            {
                await context.Response.WriteAsync(apiError.Message);
            }
        }
        catch (ApiErrorConfirmationException apiError)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = apiError.ErrorCode;
            context.Response.Headers.Append("Translation-Code", apiError.TranslationCode.ToString());

            await context.Response.WriteAsync(apiError.Message);
        }
    }
}