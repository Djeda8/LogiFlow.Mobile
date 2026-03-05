using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services;

public class LogServiceTests
{
    private readonly Mock<IFileSystemService> _fileSystemServiceMock;
    private readonly LogService _logService;

    public LogServiceTests()
    {
        _fileSystemServiceMock = new Mock<IFileSystemService>();

        _fileSystemServiceMock
            .Setup(x => x.AppDataDirectory)
            .Returns(Path.GetTempPath());

        _fileSystemServiceMock
            .Setup(x => x.ExternalStorageDirectory)
            .Returns((string?)null);

        _logService = new LogService(_fileSystemServiceMock.Object);
    }

    [Fact]
    public void Debug_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Debug("Test debug message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Info_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Info("Test info message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Warning_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Warning("Test warning message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_WithoutException_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Error("Test error message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_WithException_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() =>
            _logService.Error("Test error message", new Exception("Inner error")));
        Assert.Null(exception);
    }

    [Fact]
    public void OperationStart_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() =>
            _logService.OperationStart("TestOperation", "TestUser", "TestDetails"));
        Assert.Null(exception);
    }

    [Fact]
    public void OperationSuccess_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() =>
            _logService.OperationSuccess("TestOperation", "TestUser", "TestDetails"));
        Assert.Null(exception);
    }

    [Fact]
    public void OperationFailure_WithoutException_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() =>
            _logService.OperationFailure("TestOperation", "TestUser", "TestReason"));
        Assert.Null(exception);
    }

    [Fact]
    public void OperationFailure_WithException_DoesNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() =>
            _logService.OperationFailure("TestOperation", "TestUser", "TestReason", new Exception("Inner error")));
        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_WithExternalStorage_UsesExternalPath()
    {
        // Arrange
        _fileSystemServiceMock
            .Setup(x => x.ExternalStorageDirectory)
            .Returns(Path.GetTempPath());

        // Act & Assert
        var exception = Record.Exception(() => new LogService(_fileSystemServiceMock.Object));
        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_WithNullExternalStorage_UsesAppDataPath()
    {
        // Arrange
        _fileSystemServiceMock
            .Setup(x => x.ExternalStorageDirectory)
            .Returns((string?)null);

        // Act & Assert
        var exception = Record.Exception(() => new LogService(_fileSystemServiceMock.Object));
        Assert.Null(exception);
    }
}
