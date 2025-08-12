using FluentValidation;
using TodoApp.Application.Common.Dtos;

namespace TodoApp.Api.Common.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        
        var (statusCode, errors) = ex switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, new[] { ex.Message }),
            ValidationException validationEx => (StatusCodes.Status400BadRequest, 
                validationEx.Errors.Select(e => e.ErrorMessage).ToArray()),
            _ => (StatusCodes.Status500InternalServerError, new[] { ex.Message })
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(Result.Failure(errors));
    }
}
