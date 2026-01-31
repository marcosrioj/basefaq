using BaseFaq.Faq.Infrastructure.ApiErrorHandling.Middleware;
using Microsoft.AspNetCore.Builder;

namespace BaseFaq.Faq.Infrastructure.ApiErrorHandling.Extensions;

public static class ApiErrorHandlingServiceCollectionExtension
{
    public static void UseApiErrorHandlingMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ApiErrorHandlingMiddleware>();
    }
}