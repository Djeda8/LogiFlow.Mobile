using LogiFlow.Mobile.ViewModels.Menu;

namespace LogiFlow.Mobile.Views.Menu;

/// <summary>
/// The main menu page view. Displays the available modules for navigation.
/// </summary>
public partial class MenuPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MenuPage"/> class.
    /// </summary>
    /// <param name="viewModel">The menu view model.</param>
    public MenuPage(MenuViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
