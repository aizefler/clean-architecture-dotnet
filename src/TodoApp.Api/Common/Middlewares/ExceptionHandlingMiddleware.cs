using FluentValidation;
using TodoApp.Application.Common.Dtos;

namespace TodoApp.Api.Common.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var errors = new string[] { ex.Message };

            if (ex is ValidationException)
            {
                var validationException = (ValidationException)ex;
                errors = validationException.Errors
                    .Select(e => $"{e.ErrorMessage}")
                    .ToArray();
            }

            await context.Response.WriteAsJsonAsync(Result.Failure(errors));
        }
    }
}
