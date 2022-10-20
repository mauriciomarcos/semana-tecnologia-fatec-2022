using System.Net;
using System.Text.Json;

namespace API.Identity.Middlewares
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var result = JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                Type = "Server error",
                Details = exception.Message
            });

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            return httpContext.Response.WriteAsync(result);
        }
    }
}