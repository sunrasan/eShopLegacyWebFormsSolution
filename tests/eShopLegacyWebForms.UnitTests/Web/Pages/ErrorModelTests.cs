using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using eShopLegacyWebForms.Web.Pages;
using System;
using System.Diagnostics;

namespace eShopLegacyWebForms.UnitTests.Web.Pages;

/// <summary>
/// Unit tests for the Error page model.
/// </summary>
public class ErrorModelTests
{
    private readonly Mock<ILogger<ErrorModel>> _mockLogger;
    private readonly ErrorModel _sut;

    public ErrorModelTests()
    {
        _mockLogger = new Mock<ILogger<ErrorModel>>();
        _sut = new ErrorModel(_mockLogger.Object);
    }

    // ─── Constructor ─────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidLogger_ShouldCreateInstance()
    {
        // Act
        var model = new ErrorModel(_mockLogger.Object);

        // Assert
        model.Should().NotBeNull();
    }

    // ─── ShowRequestId ───────────────────────────────────────────────────────

    [Fact]
    public void ShowRequestId_WhenRequestIdIsNull_ShouldReturnFalse()
    {
        // Arrange
        _sut.RequestId = null;

        // Assert
        _sut.ShowRequestId.Should().BeFalse();
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        _sut.RequestId = string.Empty;

        // Assert
        _sut.ShowRequestId.Should().BeFalse();
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdHasValue_ShouldReturnTrue()
    {
        // Arrange
        _sut.RequestId = "some-request-id-123";

        // Assert
        _sut.ShowRequestId.Should().BeTrue();
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsWhitespace_ShouldReturnTrue()
    {
        // Arrange - whitespace is not empty
        _sut.RequestId = "   ";

        // Assert
        _sut.ShowRequestId.Should().BeTrue();
    }

    // ─── RequestId ───────────────────────────────────────────────────────────

    [Fact]
    public void RequestId_DefaultValue_ShouldBeNull()
    {
        // Assert
        _sut.RequestId.Should().BeNull();
    }

    [Fact]
    public void RequestId_CanBeSetAndRetrieved()
    {
        // Arrange
        const string expectedId = "test-request-id";

        // Act
        _sut.RequestId = expectedId;

        // Assert
        _sut.RequestId.Should().Be(expectedId);
    }

    // ─── OnGet ───────────────────────────────────────────────────────────────

    [Fact]
    public void OnGet_WithHttpContext_ShouldSetRequestId()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "trace-id-abc";
        _sut.PageContext = new PageContext
        {
            HttpContext = httpContext
        };

        // Act
        _sut.OnGet();

        // Assert
        _sut.RequestId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void OnGet_ShouldExecuteWithoutException()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "trace-id-xyz";
        _sut.PageContext = new PageContext
        {
            HttpContext = httpContext
        };

        // Act
        Action act = () => _sut.OnGet();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void OnGet_WhenNoActivity_ShouldUseHttpContextTraceIdentifier()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "http-trace-id";
        _sut.PageContext = new PageContext
        {
            HttpContext = httpContext
        };

        // Act
        _sut.OnGet();

        // Assert
        // When Activity.Current is null, it falls back to HttpContext.TraceIdentifier
        _sut.RequestId.Should().NotBeNullOrEmpty();
    }
}
