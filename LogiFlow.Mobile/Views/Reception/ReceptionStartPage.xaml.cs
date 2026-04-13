using LogiFlow.Mobile.ViewModels.Reception;

namespace LogiFlow.Mobile.Views.Reception;

/// <summary>
/// Reception start page. Entry point for the reception flow.
/// </summary>
public partial class ReceptionStartPage : ContentPage
{
    private readonly ReceptionStartViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionStartPage"/> class.
    /// </summary>
    /// <param name="viewModel">The reception start view model.</param>
    public ReceptionStartPage(ReceptionStartViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// Called when the page appears. Invokes the corresponding ViewModel logic.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }
}
