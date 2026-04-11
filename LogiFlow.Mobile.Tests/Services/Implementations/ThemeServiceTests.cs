using LogiFlow.Mobile.Services.Implementations;

namespace LogiFlow.Mobile.Tests.Services;

/// <summary>
/// Unit tests for <see cref="ThemeService"/>.
/// Tests focus on state management and event firing.
/// Behavior dependent on Application.Current is not testable in unit tests.
/// </summary>
public class ThemeServiceTests
{
    private readonly ThemeService _sut;

    public ThemeServiceTests()
    {
        _sut = new ThemeService();
    }

    // --- Initial state ---

    [Fact]
    public void InitialState_CurrentThemeIsLight()
    {
        Assert.Equal("light", _sut.CurrentTheme);
    }

    // --- ApplyTheme ---

    [Fact]
    public void ApplyTheme_WhenDark_SetCurrentThemeToDark()
    {
        _sut.ApplyTheme("dark");

        Assert.Equal("dark", _sut.CurrentTheme);
    }

    [Fact]
    public void ApplyTheme_WhenLight_SetsCurrentThemeToLight()
    {
        _sut.ApplyTheme("dark");
        _sut.ApplyTheme("light");

        Assert.Equal("light", _sut.CurrentTheme);
    }

    [Fact]
    public void ApplyTheme_WhenCalled_FiresThemeChangedEvent()
    {
        var eventFired = false;
        _sut.ThemeChanged += (_, _) => eventFired = true;

        _sut.ApplyTheme("dark");

        Assert.True(eventFired);
    }

    [Fact]
    public void ApplyTheme_WhenCalledMultipleTimes_FiresThemeChangedEachTime()
    {
        var eventCount = 0;
        _sut.ThemeChanged += (_, _) => eventCount++;

        _sut.ApplyTheme("dark");
        _sut.ApplyTheme("light");
        _sut.ApplyTheme("dark");

        Assert.Equal(3, eventCount);
    }

    [Fact]
    public void ApplyTheme_WhenNoSubscribers_DoesNotThrow()
    {
        var exception = Record.Exception(() => _sut.ApplyTheme("dark"));

        Assert.Null(exception);
    }

    [Fact]
    public void ApplyTheme_WhenSameThemeAppliedTwice_UpdatesCurrentTheme()
    {
        _sut.ApplyTheme("dark");
        _sut.ApplyTheme("dark");

        Assert.Equal("dark", _sut.CurrentTheme);
    }

    // --- ThemeChanged event ---

    [Fact]
    public void ThemeChanged_WhenSubscribed_ReceivesSenderAsThemeService()
    {
        object? receivedSender = null;
        _sut.ThemeChanged += (sender, _) => receivedSender = sender;

        _sut.ApplyTheme("dark");

        Assert.Same(_sut, receivedSender);
    }

    [Fact]
    public void ThemeChanged_WhenUnsubscribed_DoesNotFire()
    {
        var eventFired = false;

        void Handler(object? s, EventArgs e) => eventFired = true;

        _sut.ThemeChanged += Handler;
        _sut.ThemeChanged -= Handler;

        _sut.ApplyTheme("dark");

        Assert.False(eventFired);
    }
}
