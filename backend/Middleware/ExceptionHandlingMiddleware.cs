
using System.Text.Json;

namespace TodoApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); //يروح للcontroller
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500; 
            context.Response.ContentType = "application/json"; 

            var response = new
            {
                message = "An unexpected error occurred."
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response) //نحول الـ C# object إلى JSON //نكتب هذا الـ JSON داخل الـ HTTP Response ونرسله للمستخدم
            );
        }
    }
}