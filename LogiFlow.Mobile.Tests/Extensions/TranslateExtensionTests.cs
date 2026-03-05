using LogiFlow.Mobile.Extensions;
using Moq;

namespace LogiFlow.Mobile.Tests.Extensions;

public class TranslateExtensionTests
{
    [Fact]
    public void Key_DefaultValue_IsEmptyString()
    {
        // Assert
        Assert.Equal(string.Empty, new TranslateExtension().Key);
    }

    [Fact]
    public void Key_CanBeSet()
    {
        // Arrange
        var extension = new TranslateExtension();

        // Act
        extension.Key = "SettingsTitle";

        // Assert
        Assert.Equal("SettingsTitle", extension.Key);
    }

    [Fact]
    public void ProvideValue_WithEmptyKey_ReturnsEmptyBinding()
    {
        // Arrange
        var extension = new TranslateExtension { Key = string.Empty };
        var serviceProviderMock = new Mock<IServiceProvider>();

        // Act
        var result = extension.ProvideValue(serviceProviderMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Binding>(result);
    }

    [Fact]
    public void ProvideValue_WithKeyButNoMauiContext_ReturnsFallbackBinding()
    {
        // Arrange
        var extension = new TranslateExtension { Key = "SettingsTitle" };
        var serviceProviderMock = new Mock<IServiceProvider>();

        // Act
        var result = extension.ProvideValue(serviceProviderMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Binding>(result);
        var binding = (Binding)result;
        Assert.Equal("[SettingsTitle]", binding.Source);
    }
}
