using BallastLane.Test.Application.Common;
using BallastLane.Test.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace BallastLane.Test.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate              _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next   = next;
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
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                ValidationException         => (HttpStatusCode.BadRequest,          exception.Message),
                NotFoundException           => (HttpStatusCode.NotFound,            exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized,        exception.Message),
                InvalidOperationException   => (HttpStatusCode.BadRequest,          exception.Message),
                _                           => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)statusCode;

            var body = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data    = null!
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(body, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        }
    }
}
