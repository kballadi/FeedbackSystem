using FeedbackSystem.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FeedbackSystem.Api.Middlewares
{
    /// <summary>
    /// This is MiddleWare class to customise the Unhandled excpetions
    /// </summary>
    public class ExceptionHandlingMiddlerWare : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddlerWare> _logger;
        /// <summary>
        /// Constructor of Middleware
        /// </summary>
        /// <param name="logger">Logger to log details</param>
        public ExceptionHandlingMiddlerWare(ILogger<ExceptionHandlingMiddlerWare> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Interface method to Invoke the RequestDelegate for HttpPipeline.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (UserFeedbackSubmittedException ex)
            {
                _logger.LogError($"User Feedback Submitted Exception: {ex.Message}");
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(new ErrorDetails
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                }.ToString());
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Exception caught {ex.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(new ErrorDetails
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Internal Server Error. Please try again later."
                }.ToString());
            }
        }
    }
}
