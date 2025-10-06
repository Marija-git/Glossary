using Glossary.BusinessLogic.Exceptions;
using System.Net;
using System.Text.Json;

namespace Glossary.API.Middlewares
{
    public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (NotFoundException notFoundEx)
            {
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, notFoundEx.Message);
            }
            catch (ForbidException forbidEx)
            {
                await HandleExceptionAsync(context, HttpStatusCode.Forbidden, forbidEx.Message);
            }
            catch (BadRequestException badRequestEx)
            {
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, badRequestEx.Message);
            }
            catch (UnauthorizedException unauthorizedEx)
            {
                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, unauthorizedEx.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Something went wrong.");
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var problem = new
            {
                status = (int)statusCode,
                error = message,
                path = context.Request.Path,
                timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
