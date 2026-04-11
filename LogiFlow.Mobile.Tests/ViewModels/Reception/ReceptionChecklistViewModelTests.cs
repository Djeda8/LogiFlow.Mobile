using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Reception;
using Moq;

namespace LogiFlow.Mobile.Tests.ViewModels.Reception;

/// <summary>
/// Unit tests for <see cref="ReceptionChecklistViewModel"/>.
/// Covers checklist loading, item completion, flow-specific items and navigation.
/// </summary>
public class ReceptionChecklistViewModelTests
{
    private readonly Mock<IReceptionSessionService> _receptionSessionServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<IErrorHandlerService> _errorHandlerServiceMock;
    private readonly ReceptionChecklistViewModel _sut;

    public ReceptionChecklistViewModelTests()
    {
        _receptionSessionServiceMock = new Mock<IReceptionSessionService>();
        _navigationServiceMock = new Mock<INavigationService>();
        _localizationServiceMock = new Mock<ILocalizationService>();
        _logServiceMock = new Mock<ILogService>();
        _errorHandlerServiceMock = new Mock<IErrorHandlerService>();

        _localizationServiceMock
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => key);

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<Exception>(), It.IsAny<string>()))
            .Returns("Unexpected error");

        _sut = new ReceptionChecklistViewModel(
            _receptionSessionServiceMock.Object,
            _navigationServiceMock.Object,
            _localizationServiceMock.Object,
            _logServiceMock.Object,
            _errorHandlerServiceMock.Object);
    }

    // --- Initial state ---

    [Fact]
    public void InitialState_ChecklistItemsIsEmpty()
    {
        Assert.Empty(_sut.ChecklistItems);
    }

    [Fact]
    public void InitialState_AllItemsCheckedIsFalse()
    {
        Assert.False(_sut.AllItemsChecked);
    }

    [Fact]
    public void InitialState_ContinueCommandIsDisabled()
    {
        Assert.False(_sut.ContinueCommand.CanExecute(null));
    }

    // --- Load ---

    [Fact]
    public void Load_WhenNoActiveSession_SetsHasError()
    {
        _receptionSessionServiceMock
            .Setup(x => x.CurrentReception)
            .Returns((ReceptionDto?)null);

        _sut.Load();

        Assert.True(_sut.HasError);
    }

    [Fact]
    public void Load_WhenNoActiveSession_SetsErrorMessage()
    {
        _receptionSessionServiceMock
            .Setup(x => x.CurrentReception)
            .Returns((ReceptionDto?)null);

        _sut.Load();

        Assert.NotEmpty(_sut.ErrorMessage);
    }

    [Fact]
    public void Load_WhenSessionActive_SetsReceptionNumber()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));

        _sut.Load();

        Assert.Equal("REC-001", _sut.ReceptionNumber);
    }

    [Fact]
    public void Load_WhenSessionActive_SetsFlowType()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));

        _sut.Load();

        Assert.Equal(ReceptionFlowType.Standard, _sut.FlowType);
    }

    // --- Checklist items by flow type ---

    [Fact]
    public void Load_ForStandardFlow_LoadsThreeCommonItems()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));

        _sut.Load();

        Assert.Equal(3, _sut.ChecklistItems.Count);
    }

    [Fact]
    public void Load_ForTestSampleFlow_LoadsSixItems()
    {
        // 3 common + 3 test sample specific
        SetupSession(BuildReception("REC-002", ReceptionFlowType.TestSample));

        _sut.Load();

        Assert.Equal(6, _sut.ChecklistItems.Count);
    }

    [Fact]
    public void Load_WhenReceptionHasVehicleArticle_LoadsExtraVehicleItems()
    {
        // 3 common + 2 vehicle specific
        var reception = BuildReception("REC-001", ReceptionFlowType.Standard);
        reception.DetailLines.Add(new ReceptionDetailDto
        {
            Article = "VEHICLE",
            Quantity = 1,
            Location = "A-01-01",
        });
        SetupSession(reception);

        _sut.Load();

        Assert.Equal(5, _sut.ChecklistItems.Count);
    }

    [Fact]
    public void Load_WhenTestSampleWithVehicle_LoadsAllNineItems()
    {
        // 3 common + 3 test sample + 2 vehicle = 8
        var reception = BuildReception("REC-002", ReceptionFlowType.TestSample);
        reception.DetailLines.Add(new ReceptionDetailDto
        {
            Article = "VEHICLE",
            Quantity = 1,
            Location = "QA-ZONE-01",
        });
        SetupSession(reception);

        _sut.Load();

        Assert.Equal(8, _sut.ChecklistItems.Count);
    }

    [Fact]
    public void Load_WhenCalledTwice_ClearsAndRebuildsChecklist()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));

        _sut.Load();
        _sut.Load();

        Assert.Equal(3, _sut.ChecklistItems.Count);
    }

    // --- AllItemsChecked ---

    [Fact]
    public void AllItemsChecked_WhenNoItemsChecked_IsFalse()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        _sut.Load();

        Assert.False(_sut.AllItemsChecked);
    }

    [Fact]
    public void AllItemsChecked_WhenAllItemsChecked_IsTrue()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        _sut.Load();

        foreach (var item in _sut.ChecklistItems)
            item.IsChecked = true;

        Assert.True(_sut.AllItemsChecked);
    }

    [Fact]
    public void AllItemsChecked_WhenPartialItemsChecked_IsFalse()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        _sut.Load();

        _sut.ChecklistItems.First().IsChecked = true;

        Assert.False(_sut.AllItemsChecked);
    }

    // --- ContinueCommand CanExecute ---

    [Fact]
    public void ContinueCommand_WhenAllItemsChecked_IsEnabled()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        _sut.Load();

        foreach (var item in _sut.ChecklistItems)
            item.IsChecked = true;

        Assert.True(_sut.ContinueCommand.CanExecute(null));
    }

    [Fact]
    public void ContinueCommand_WhenNotAllItemsChecked_IsDisabled()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        _sut.Load();

        _sut.ChecklistItems.First().IsChecked = true;

        Assert.False(_sut.ContinueCommand.CanExecute(null));
    }

    // --- ContinueAsync ---

    [Fact]
    public async Task ContinueAsync_WhenAllItemsChecked_NavigatesToConfirmationPage()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        _sut.Load();

        foreach (var item in _sut.ChecklistItems)
            item.IsChecked = true;

        await _sut.ContinueCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Reception.ReceptionConfirmationPage),
            false), Times.Once);
    }

    [Fact]
    public async Task ContinueAsync_WhenNavigationThrows_SetsHasError()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        _sut.Load();

        foreach (var item in _sut.ChecklistItems)
            item.IsChecked = true;

        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new Exception("Navigation error"));

        await _sut.ContinueCommand.ExecuteAsync(null);

        Assert.True(_sut.HasError);
    }

    // --- RefreshAllItemsChecked ---

    [Fact]
    public void RefreshAllItemsChecked_WhenItemChecked_UpdatesAllItemsChecked()
    {
        SetupSession(BuildReception("REC-001", ReceptionFlowType.Standard));
        _sut.Load();

        foreach (var item in _sut.ChecklistItems)
            item.IsChecked = true;

        _sut.RefreshAllItemsChecked();

        Assert.True(_sut.AllItemsChecked);
    }

    [Fact]
    public void RefreshAllItemsChecked_WhenNoItems_AllItemsCheckedIsFalse()
    {
        _sut.RefreshAllItemsChecked();

        Assert.False(_sut.AllItemsChecked);
    }

    // --- GoBackAsync ---

    [Fact]
    public async Task GoBackAsync_NavigatesBack()
    {
        await _sut.GoBackCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateBackAsync(false), Times.Once);
    }

    // --- Helpers ---

    private static ReceptionDto BuildReception(string number, string flowType) =>
        new()
        {
            ReceptionNumber = number,
            FlowType = flowType,
            Status = ReceptionStatus.New,
            Header = new ReceptionHeaderDto
            {
                Sender = "Supplier A",
                Recipient = "Warehouse 1",
            },
        };

    private void SetupSession(ReceptionDto reception) =>
        _receptionSessionServiceMock
            .Setup(x => x.CurrentReception)
            .Returns(reception);
}
