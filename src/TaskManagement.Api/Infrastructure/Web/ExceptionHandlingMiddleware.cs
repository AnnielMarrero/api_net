using System.Net;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Application.Exceptions;

namespace TaskManagement.Api.Infrastructure.Web;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    IProblemDetailsService problemDetailsService,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (TaskNotFoundException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await WriteProblemDetailsAsync(context, exception.Message, StatusCodes.Status404NotFound);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await WriteProblemDetailsAsync(
                context,
                "An unexpected error occurred.",
                StatusCodes.Status500InternalServerError);
        }
    }

    private async Task WriteProblemDetailsAsync(HttpContext context, string detail, int statusCode)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode == StatusCodes.Status404NotFound ? "Resource not found" : "Server error",
            Detail = detail,
            Instance = context.Request.Path
        };

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problem
        });
    }
}
