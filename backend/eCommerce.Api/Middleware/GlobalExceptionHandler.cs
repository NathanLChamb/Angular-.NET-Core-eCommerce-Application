using eCommerce.Application.Constants;
using eCommerce.Application.Exceptions;
using System.Net;

namespace eCommerce.Api.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly RequestDelegate _next;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var requestId = context.TraceIdentifier;
                _logger.LogError(ex, "Request {RequestId} failed with exception {ExceptionType}", context.TraceIdentifier, ex.GetType().Name);

                var response = ex switch
                {
                    NotFoundException nf => new ErrorResponse(nf.Code, nf.Message, DateTime.UtcNow, requestId),
                    ValidationRuleException vr => new ErrorResponse(vr.Code, vr.Message, DateTime.UtcNow, requestId),
                    BusinessRuleException br => new ErrorResponse(br.Code, br.Message, DateTime.UtcNow, requestId),
                    ConflictException ce => new ErrorResponse(ce.Code, ce.Message, DateTime.UtcNow, requestId),
                    _ => new ErrorResponse("INTERNAL_ERROR", "An unexpected error occurred", DateTime.UtcNow, requestId)
                };

                HttpStatusCode statusCode = ex switch
                {
                    NotFoundException => HttpStatusCode.NotFound,
                    ValidationRuleException => HttpStatusCode.BadRequest,
                    BusinessRuleException => HttpStatusCode.BadRequest,
                    ConflictException => HttpStatusCode.Conflict,
                    _ => HttpStatusCode.InternalServerError
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;
                context.Response.Headers["X-Request-ID"] = requestId;

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
