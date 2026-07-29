namespace Pdr.Api.Middleware;

public class SlidingSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            if (context.User.Identity?.IsAuthenticated == true && context.Request.Cookies.TryGetValue("pdr_jwt", out var token))
            {
                context.Response.Cookies.Append("pdr_jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                });
            }

            return Task.CompletedTask;
        });

        await next(context);
    }
}
