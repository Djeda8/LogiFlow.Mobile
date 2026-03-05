using LogiFlow.Mobile.ViewModels.Splash;

namespace LogiFlow.Mobile.Views.Splash;

/// <summary>
/// The splash screen page. Displays briefly on startup while validating the user session.
/// </summary>
public partial class SplashPage : ContentPage
{
    private readonly SplashViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="SplashPage"/> class.
    /// </summary>
    /// <param name="viewModel">The splash screen view model.</param>
    public SplashPage(SplashViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <inheritdoc/>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.StartAsync();
    }
}
