using LogiFlow.Mobile.ViewModels.Reception;

namespace LogiFlow.Mobile.Views.Reception;

/// <summary>
/// Reception confirmation page. Allows the operator to confirm or reject the reception.
/// </summary>
public partial class ReceptionConfirmationPage : ContentPage
{
    private readonly ReceptionConfirmationViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionConfirmationPage"/> class.
    /// </summary>
    /// <param name="viewModel">The reception confirmation view model.</param>
    public ReceptionConfirmationPage(ReceptionConfirmationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <inheritdoc/>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Load();
    }
}
