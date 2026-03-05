using LogiFlow.Mobile.ViewModels.Settings;

namespace LogiFlow.Mobile.Views.Settings;

/// <summary>
/// The settings page view. Allows the user to configure application and hardware parameters.
/// </summary>
public partial class SettingsPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    /// <param name="viewModel">The settings view model.</param>
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
