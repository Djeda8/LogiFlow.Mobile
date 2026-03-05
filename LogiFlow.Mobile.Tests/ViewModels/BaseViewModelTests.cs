using LogiFlow.Mobile.ViewModels.Base;

namespace LogiFlow.Mobile.Tests.ViewModels;

public class BaseViewModelTests
{
    private readonly BaseViewModel _viewModel;

    public BaseViewModelTests()
    {
        _viewModel = new ConcreteViewModel();
    }

    [Fact]
    public void IsNotBusy_WhenIsBusyFalse_ReturnsTrue()
    {
        // Arrange
        _viewModel.IsBusy = false;

        // Act & Assert
        Assert.True(_viewModel.IsNotBusy);
    }

    [Fact]
    public void IsNotBusy_WhenIsBusyTrue_ReturnsFalse()
    {
        // Arrange
        _viewModel.IsBusy = true;

        // Act & Assert
        Assert.False(_viewModel.IsNotBusy);
    }

    [Fact]
    public void IsNotBusy_ChangesWhenIsBusyChanges()
    {
        // Arrange
        _viewModel.IsBusy = false;
        Assert.True(_viewModel.IsNotBusy);

        // Act
        _viewModel.IsBusy = true;

        // Assert
        Assert.False(_viewModel.IsNotBusy);
    }

    [Fact]
    public void IsBusy_DefaultValue_IsFalse()
    {
        // Assert
        Assert.False(_viewModel.IsBusy);
    }

    [Fact]
    public void HasError_DefaultValue_IsFalse()
    {
        // Assert
        Assert.False(_viewModel.HasError);
    }

    [Fact]
    public void ErrorMessage_DefaultValue_IsEmptyString()
    {
        // Assert
        Assert.Equal(string.Empty, _viewModel.ErrorMessage);
    }

    [Fact]
    public void IsNotBusy_FiresPropertyChanged_WhenIsBusyChanges()
    {
        // Arrange
        var propertyChangedFired = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(BaseViewModel.IsNotBusy))
            {
                propertyChangedFired = true;
            }
        };

        // Act
        _viewModel.IsBusy = true;

        // Assert
        Assert.True(propertyChangedFired);
    }

    /// <summary>
    /// Concrete implementation of BaseViewModel for testing purposes.
    /// </summary>
    private class ConcreteViewModel : BaseViewModel
    {
    }
}
