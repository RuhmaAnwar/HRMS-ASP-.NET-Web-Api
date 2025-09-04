using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Net;
using System.Text.Json;

namespace HRMS.Middleware
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
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

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";

            switch (exception)
            {
                case ConflictException ce:
                    statusCode = HttpStatusCode.Conflict;
                    message = ce.Message;
                    break;

                case ValidationException ve:
                    statusCode = HttpStatusCode.BadRequest;
                    message = ve.Message;
                    break;

                case ArgumentException or InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Forbidden;
                    message = exception.Message;
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
                Details = _env.IsDevelopment() ? exception.StackTrace : null // Hide stack trace in production
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
