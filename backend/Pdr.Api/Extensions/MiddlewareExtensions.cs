using Microsoft.AspNetCore.Builder;
using Pdr.Api.Middleware;

namespace Pdr.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Prevent browser back button caching for authenticated API responses
        app.Use(async (context, next) =>
        {
            context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";

            await next();
        });

        return app;
    }
}
