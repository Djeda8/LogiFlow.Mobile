using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services.Implementations;

/// <summary>
/// Unit tests for <see cref="LogService"/>.
/// </summary>
public class LogServiceTests
{
    private readonly LogService _sut;

    public LogServiceTests()
    {
        var fileSystemServiceMock = new Mock<IFileSystemService>();
        fileSystemServiceMock
            .Setup(x => x.AppDataDirectory)
            .Returns(Path.GetTempPath());

        _sut = new LogService(fileSystemServiceMock.Object);
    }

    [Fact]
    public void Constructor_DoesNotThrow()
    {
        var fileSystemServiceMock = new Mock<IFileSystemService>();
        fileSystemServiceMock
            .Setup(x => x.AppDataDirectory)
            .Returns(Path.GetTempPath());

        var exception = Record.Exception(() => new LogService(fileSystemServiceMock.Object));
        Assert.Null(exception);
    }

    [Fact]
    public void Debug_DoesNotThrow()
    {
        var exception = Record.Exception(() => _sut.Debug("Test debug message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Debug_WithProperties_DoesNotThrow()
    {
        var exception = Record.Exception(() => _sut.Debug("Test {Prop}", "value"));
        Assert.Null(exception);
    }

    [Fact]
    public void Info_DoesNotThrow()
    {
        var exception = Record.Exception(() => _sut.Info("Test info message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Info_WithProperties_DoesNotThrow()
    {
        var exception = Record.Exception(() => _sut.Info("Test {Prop}", "value"));
        Assert.Null(exception);
    }

    [Fact]
    public void Warning_DoesNotThrow()
    {
        var exception = Record.Exception(() => _sut.Warning("Test warning message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Warning_WithProperties_DoesNotThrow()
    {
        var exception = Record.Exception(() => _sut.Warning("Test {Prop}", "value"));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_WithoutException_DoesNotThrow()
    {
        var exception = Record.Exception(() => _sut.Error("Test error message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_WithException_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
            _sut.Error("Test error message", new Exception("Inner error")));
        Assert.Null(exception);
    }

    [Fact]
    public void OperationStart_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
            _sut.OperationStart("TestOperation", "TestUser", "TestDetails"));
        Assert.Null(exception);
    }

    [Fact]
    public void OperationStart_WithNullDetails_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
            _sut.OperationStart("TestOperation", "TestUser"));
        Assert.Null(exception);
    }

    [Fact]
    public void OperationSuccess_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
            _sut.OperationSuccess("TestOperation", "TestUser", "TestDetails"));
        Assert.Null(exception);
    }

    [Fact]
    public void OperationSuccess_WithNullDetails_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
            _sut.OperationSuccess("TestOperation", "TestUser"));
        Assert.Null(exception);
    }

    [Fact]
    public void OperationFailure_WithoutException_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
            _sut.OperationFailure("TestOperation", "TestUser", "TestReason"));
        Assert.Null(exception);
    }

    [Fact]
    public void OperationFailure_WithException_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
            _sut.OperationFailure("TestOperation", "TestUser", "TestReason",
                new Exception("Inner error")));
        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_WithExternalStorage_DoesNotThrow()
    {
        var fileSystemServiceMock = new Mock<IFileSystemService>();
        fileSystemServiceMock
            .Setup(x => x.AppDataDirectory)
            .Returns(Path.GetTempPath());

        var exception = Record.Exception(() => new LogService(fileSystemServiceMock.Object));
        Assert.Null(exception);
    }
}
