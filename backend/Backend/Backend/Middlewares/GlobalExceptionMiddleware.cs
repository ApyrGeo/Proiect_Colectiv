using System.Net;
using Backend.Exceptions;
using log4net;

namespace Backend.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILog _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILog logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.Error("An unexpected error occurred.");

        //More log stuff        

        ExceptionResponse response = exception switch
        {
            NotFoundException _ => new ExceptionResponse(HttpStatusCode.NotFound, "The request key not found."),
            _ => new ExceptionResponse(HttpStatusCode.InternalServerError, "Internal server error. Please retry later.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}
