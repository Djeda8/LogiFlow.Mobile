using LogiFlow.Mobile.ViewModels.Login;

namespace LogiFlow.Mobile.Views.Login;

/// <summary>
/// The login page view. Handles user authentication entry point.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public partial class LoginPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginPage"/> class.
    /// </summary>
    /// <param name="viewModel">The login view model.</param>
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
