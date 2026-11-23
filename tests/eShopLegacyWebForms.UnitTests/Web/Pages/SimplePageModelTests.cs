using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using eShopLegacyWebForms.Web.Pages;
using System;

namespace eShopLegacyWebForms.UnitTests.Web.Pages;

/// <summary>
/// Unit tests for the home Index page model.
/// </summary>
public class IndexModelTests
{
    private readonly Mock<ILogger<IndexModel>> _mockLogger;
    private readonly IndexModel _sut;

    public IndexModelTests()
    {
        _mockLogger = new Mock<ILogger<IndexModel>>();
        _sut = new IndexModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldCreateInstance()
    {
        // Act
        var model = new IndexModel(_mockLogger.Object);

        // Assert
        model.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldNotThrow()
    {
        // Act - IndexModel does not guard against null logger
        var model = new IndexModel(null!);

        // Assert
        model.Should().NotBeNull();
    }

    [Fact]
    public void OnGet_ShouldExecuteWithoutException()
    {
        // Act
        Action act = () => _sut.OnGet();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void OnGet_ShouldLogInformation()
    {
        // Act
        _sut.OnGet();

        // Assert - verify logger was called (at least once)
        _mockLogger.Invocations.Should().NotBeEmpty();
    }
}

/// <summary>
/// Unit tests for the About page model.
/// </summary>
public class AboutModelTests
{
    private readonly Mock<ILogger<AboutModel>> _mockLogger;
    private readonly AboutModel _sut;

    public AboutModelTests()
    {
        _mockLogger = new Mock<ILogger<AboutModel>>();
        _sut = new AboutModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldCreateInstance()
    {
        // Act
        var model = new AboutModel(_mockLogger.Object);

        // Assert
        model.Should().NotBeNull();
    }

    [Fact]
    public void OnGet_ShouldExecuteWithoutException()
    {
        // Act
        Action act = () => _sut.OnGet();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void OnGet_ShouldLogInformation()
    {
        // Act
        _sut.OnGet();

        // Assert
        _mockLogger.Invocations.Should().NotBeEmpty();
    }
}

/// <summary>
/// Unit tests for the Contact page model.
/// </summary>
public class ContactModelTests
{
    private readonly Mock<ILogger<ContactModel>> _mockLogger;
    private readonly ContactModel _sut;

    public ContactModelTests()
    {
        _mockLogger = new Mock<ILogger<ContactModel>>();
        _sut = new ContactModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldCreateInstance()
    {
        // Act
        var model = new ContactModel(_mockLogger.Object);

        // Assert
        model.Should().NotBeNull();
    }

    [Fact]
    public void OnGet_ShouldExecuteWithoutException()
    {
        // Act
        Action act = () => _sut.OnGet();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void OnGet_ShouldLogInformation()
    {
        // Act
        _sut.OnGet();

        // Assert
        _mockLogger.Invocations.Should().NotBeEmpty();
    }
}
