public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (path == "/User/Login" || path == "/User/Register"
            || path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/images"))
        {
            await _next(context);
            return;
        }


        //si la cookie no existe
        if (!context.Request.Cookies.ContainsKey("AuthCookie"))
        {
            context.Response.Redirect("/User/Login");
            return;
        }

        await _next(context);
    }
}
