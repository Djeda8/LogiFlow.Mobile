using LogiFlow.Mobile.Models;

namespace LogiFlow.Mobile.Tests.Models;

public class SettingsOptionTests
{
    [Fact]
    public void Equals_WithSameCode_ReturnsTrue()
    {
        // Arrange
        var option1 = new SettingsOption { Code = "light", DisplayName = "Light" };
        var option2 = new SettingsOption { Code = "light", DisplayName = "Claro" };

        // Assert
        Assert.Equal(option1, option2);
    }

    [Fact]
    public void Equals_WithDifferentCode_ReturnsFalse()
    {
        // Arrange
        var option1 = new SettingsOption { Code = "light", DisplayName = "Light" };
        var option2 = new SettingsOption { Code = "dark", DisplayName = "Dark" };

        // Assert
        Assert.NotEqual(option1, option2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var option = new SettingsOption { Code = "light", DisplayName = "Light" };

        // Assert
        Assert.False(option.Equals(null));
    }

    [Fact]
    public void GetHashCode_WithSameCode_ReturnsSameHash()
    {
        // Arrange
        var option1 = new SettingsOption { Code = "light", DisplayName = "Light" };
        var option2 = new SettingsOption { Code = "light", DisplayName = "Claro" };

        // Assert
        Assert.Equal(option1.GetHashCode(), option2.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsDisplayName()
    {
        // Arrange
        var option = new SettingsOption { Code = "light", DisplayName = "Light" };

        // Assert
        Assert.Equal("Light", option.ToString());
    }
}
