using LogiFlow.Mobile.Extensions;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Extensions;

public class TranslationWrapperTests
{
    private readonly Mock<ILocalizationService> _localizationServiceMock;

    public TranslationWrapperTests()
    {
        _localizationServiceMock = new Mock<ILocalizationService>();
        _localizationServiceMock
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => $"Translated_{key}");
    }

    [Fact]
    public void Value_ReturnsTranslatedString()
    {
        // Arrange
        var wrapper = new TranslationWrapper(_localizationServiceMock.Object, "SettingsTitle");

        // Act
        var result = wrapper.Value;

        // Assert
        Assert.Equal("Translated_SettingsTitle", result);
    }

    [Fact]
    public void Value_CallsLocalizationService()
    {
        // Arrange
        var wrapper = new TranslationWrapper(_localizationServiceMock.Object, "SettingsTitle");

        // Act
        _ = wrapper.Value;

        // Assert
        _localizationServiceMock.Verify(x => x.GetString("SettingsTitle"), Times.Once);
    }

    [Fact]
    public void LanguageChanged_FiresPropertyChanged()
    {
        // Arrange
        var wrapper = new TranslationWrapper(_localizationServiceMock.Object, "SettingsTitle");
        var propertyChangedFired = false;
        wrapper.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TranslationWrapper.Value))
            {
                propertyChangedFired = true;
            }
        };

        // Act
        _localizationServiceMock.Raise(x => x.LanguageChanged += null, EventArgs.Empty);

        // Assert
        Assert.True(propertyChangedFired);
    }

    [Fact]
    public void Value_AfterLanguageChanged_ReturnsUpdatedTranslation()
    {
        // Arrange
        var wrapper = new TranslationWrapper(_localizationServiceMock.Object, "SettingsTitle");

        _localizationServiceMock
            .Setup(x => x.GetString("SettingsTitle"))
            .Returns("Configuración");

        // Act
        _localizationServiceMock.Raise(x => x.LanguageChanged += null, EventArgs.Empty);
        var result = wrapper.Value;

        // Assert
        Assert.Equal("Configuración", result);
    }
}
