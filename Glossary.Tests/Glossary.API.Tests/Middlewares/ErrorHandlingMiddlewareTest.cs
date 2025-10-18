using FluentAssertions;
using Glossary.API.Middlewares;
using Glossary.BusinessLogic.Exceptions;
using Glossary.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Glossary.Tests.Glossary.API.Tests.Middlewares
{
    public class ErrorHandlingMiddlewareTest
    {
        private readonly Mock<ILogger<ErrorHandlingMiddleware>> _loggerMock;
        private readonly ErrorHandlingMiddleware _middleware;
        public ErrorHandlingMiddlewareTest()
        {
            _loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            _middleware = new ErrorHandlingMiddleware(_loggerMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_NoException_ShouldCallNextDelegate()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var nextDelegateMock = new Mock<RequestDelegate>();

            // Act
            await _middleware.InvokeAsync(context, nextDelegateMock.Object);

            // Assert
            nextDelegateMock.Verify(next => next.Invoke(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_NotFoundException_ShouldSetStatusCode404()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new NotFoundException(nameof(GlossaryTerm), "1");

            // Act
            await _middleware.InvokeAsync(context, _ => throw exception);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            context.Response.ContentType.Should().Be("application/json");
        }

        [Fact]
        public async Task InvokeAsync_ForbidException_ShouldSetStatusCode403()
        {
            var context = new DefaultHttpContext();
            var exception = new ForbidException();

            await _middleware.InvokeAsync(context, _ => throw exception);

            context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            context.Response.ContentType.Should().Be("application/json");
        }

        [Fact]
        public async Task InvokeAsync_GenericException_ShouldSetStatusCode500()
        {
            var context = new DefaultHttpContext();
            var exception = new Exception("Something went wrong");

            await _middleware.InvokeAsync(context, _ => throw exception);

            context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            context.Response.ContentType.Should().Be("application/json");
        }

    }
}
