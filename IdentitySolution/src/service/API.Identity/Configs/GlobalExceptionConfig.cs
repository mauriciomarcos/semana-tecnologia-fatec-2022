using API.Identity.Middlewares;

namespace API.Identity.Configs
{
    public static class GlobalExceptionConfig
    {
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}