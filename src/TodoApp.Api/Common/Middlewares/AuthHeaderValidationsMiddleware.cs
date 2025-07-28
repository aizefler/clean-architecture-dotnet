using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using TodoApp.Application.Common;
using TodoApp.Application.Common.Dtos;

namespace TodoApp.Api.Common.Middlewares;

public class AuthHeaderValidationsMiddleware
{
    private readonly RequestDelegate _next;

    public AuthHeaderValidationsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.HasValue &&
            (context.Request.Path.Value.Contains("/openapi") ||
            context.Request.Path.Value.Contains("/health")))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("x-id-token", out var token))
        {
            await WriteJsonError(context, ResultError.UsuarioNaoAutenticado);
            return;
        }

        try
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtTokenHandler.ReadJwtToken(token.FirstOrDefault());
            context.Items["Username"] = jwtToken.Claims.First(c => c.Type == "preferred_username").Value.Split("@")[0];
            context.Items["Email"] = jwtToken.Claims.First(c => c.Type == "preferred_username").Value;
            await _next(context);
        }
        catch
        {
            await WriteJsonError(context, ResultError.UsuarioNaoAutenticado);
        }
    }

    private static async Task WriteJsonError(HttpContext context, string error)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";
        var errors = new string[] { error };
        await context.Response.WriteAsync(JsonSerializer.Serialize(Result.Failure(errors)));
    }
}
