using LogiFlow.Mobile.ViewModels.Reception;

namespace LogiFlow.Mobile.Views.Reception;

/// <summary>
/// Reception checklist page. Validates operational requirements before confirmation.
/// Mandatory for TEST_SAMPLE flow and VEHICLE articles.
/// </summary>
public partial class ReceptionChecklistPage : ContentPage
{
    private readonly ReceptionChecklistViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceptionChecklistPage"/> class.
    /// </summary>
    /// <param name="viewModel">The reception checklist view model.</param>
    public ReceptionChecklistPage(ReceptionChecklistViewModel viewModel)
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
