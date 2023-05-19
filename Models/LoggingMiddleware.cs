using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var controllerName = context.Request.RouteValues["controller"];
        var actionName = context.Request.RouteValues["action"];

        _logger.LogInformation($"Entering {controllerName}.{actionName}");

        await _next(context);

        _logger.LogInformation($"Exiting {controllerName}.{actionName}");
    }
}