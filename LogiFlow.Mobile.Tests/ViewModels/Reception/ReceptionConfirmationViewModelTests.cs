using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Reception;
using Moq;

namespace LogiFlow.Mobile.Tests.ViewModels.Reception;

/// <summary>
/// Unit tests for <see cref="ReceptionConfirmationViewModel"/>.
/// Covers loading, confirmation, rejection and navigation.
/// </summary>
public class ReceptionConfirmationViewModelTests
{
    private readonly Mock<IReceptionService> _receptionServiceMock;
    private readonly Mock<IReceptionSessionService> _receptionSessionServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<IErrorHandlerService> _errorHandlerServiceMock;
    private readonly Mock<IChatDialogService> _chatDialogService;

    private readonly ReceptionConfirmationViewModel _sut;

    public ReceptionConfirmationViewModelTests()
    {
        _receptionServiceMock = new Mock<IReceptionService>();
        _receptionSessionServiceMock = new Mock<IReceptionSessionService>();
        _navigationServiceMock = new Mock<INavigationService>();
        _localizationServiceMock = new Mock<ILocalizationService>();
        _logServiceMock = new Mock<ILogService>();
        _errorHandlerServiceMock = new Mock<IErrorHandlerService>();
        _chatDialogService = new Mock<IChatDialogService>();

        _localizationServiceMock
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => key);

        _errorHandlerServiceMock
            .Setup(x => x.Handle(It.IsAny<Exception>(), It.IsAny<string>()))
            .Returns("Unexpected error");

        _sut = new ReceptionConfirmationViewModel(
            _receptionServiceMock.Object,
            _receptionSessionServiceMock.Object,
            _navigationServiceMock.Object,
            _localizationServiceMock.Object,
            _logServiceMock.Object,
            _errorHandlerServiceMock.Object,
            _chatDialogService.Object);
    }

    // --- Initial state ---

    [Fact]
    public void InitialState_IsRejectingIsFalse()
    {
        Assert.False(_sut.IsRejecting);
    }

    [Fact]
    public void InitialState_HasErrorIsFalse()
    {
        Assert.False(_sut.HasError);
    }

    [Fact]
    public void InitialState_RejectCommandIsDisabled()
    {
        Assert.False(_sut.RejectCommand.CanExecute(null));
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
        var reception = BuildReception("REC-001", 2, 10);
        SetupSession(reception);

        _sut.Load();

        Assert.Equal("REC-001", _sut.ReceptionNumber);
    }

    [Fact]
    public void Load_WhenSessionActive_SetsSender()
    {
        var reception = BuildReception("REC-001", 2, 10);
        SetupSession(reception);

        _sut.Load();

        Assert.Equal("Supplier A", _sut.Sender);
    }

    [Fact]
    public void Load_WhenSessionActive_SetsRecipient()
    {
        var reception = BuildReception("REC-001", 2, 10);
        SetupSession(reception);

        _sut.Load();

        Assert.Equal("Warehouse 1", _sut.Recipient);
    }

    [Fact]
    public void Load_WhenSessionActive_SetsTotalLines()
    {
        var reception = BuildReception("REC-001", 2, 5);
        SetupSession(reception);

        _sut.Load();

        Assert.Equal(2, _sut.TotalLines);
    }

    [Fact]
    public void Load_WhenSessionActive_SetsTotalQuantity()
    {
        var reception = BuildReception("REC-001", 2, 5);
        SetupSession(reception);

        _sut.Load();

        Assert.Equal(10, _sut.TotalQuantity);
    }

    [Fact]
    public void Load_AlwaysResetsRejectionState()
    {
        var reception = BuildReception("REC-001", 1, 5);
        SetupSession(reception);

        // Simulate previous rejection state
        _sut.StartRejectAsyncCommand.Execute(null);
        _sut.RejectionReason = "Some reason";

        _sut.Load();

        Assert.False(_sut.IsRejecting);
        Assert.Equal(string.Empty, _sut.RejectionReason);
        Assert.False(_sut.RejectionReasonHasError);
    }

    // --- ConfirmAsync ---

    [Fact]
    public async Task ConfirmAsync_WhenSuccessful_NavigatesToItemsPage()
    {
        var reception = BuildReception("REC-001", 1, 5);
        SetupSession(reception);

        _receptionServiceMock
            .Setup(x => x.ConfirmReceptionAsync(reception))
            .ReturnsAsync([new ItemDto { ItemCode = "ITEM-001" }]);

        await _sut.ConfirmCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Reception.ReceptionItemsPage),
            false), Times.Once);
    }

    [Fact]
    public async Task ConfirmAsync_WhenServiceThrows_SetsHasError()
    {
        var reception = BuildReception("REC-001", 1, 5);
        SetupSession(reception);

        _receptionServiceMock
            .Setup(x => x.ConfirmReceptionAsync(reception))
            .ThrowsAsync(new Exception("Service error"));

        await _sut.ConfirmCommand.ExecuteAsync(null);

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task ConfirmAsync_WhenComplete_IsBusyIsFalse()
    {
        var reception = BuildReception("REC-001", 1, 5);
        SetupSession(reception);

        _receptionServiceMock
            .Setup(x => x.ConfirmReceptionAsync(reception))
            .ReturnsAsync([]);

        await _sut.ConfirmCommand.ExecuteAsync(null);

        Assert.False(_sut.IsBusy);
    }

    // --- StartRejectAsync ---

    [Fact]
    public void StartRejectAsync_SetsIsRejectingTrue()
    {
        _sut.StartRejectAsyncCommand.Execute(null);

        Assert.True(_sut.IsRejecting);
    }

    [Fact]
    public void StartRejectAsync_ClearsRejectionReason()
    {
        _sut.RejectionReason = "Previous reason";
        _sut.StartRejectAsyncCommand.Execute(null);

        Assert.Equal(string.Empty, _sut.RejectionReason);
    }

    // --- CancelReject ---

    [Fact]
    public void CancelReject_SetsIsRejectingFalse()
    {
        _sut.StartRejectAsyncCommand.Execute(null);
        _sut.CancelRejectCommand.Execute(null);

        Assert.False(_sut.IsRejecting);
    }

    [Fact]
    public void CancelReject_ClearsRejectionReason()
    {
        _sut.StartRejectAsyncCommand.Execute(null);
        _sut.RejectionReason = "Some reason";
        _sut.CancelRejectCommand.Execute(null);

        Assert.Equal(string.Empty, _sut.RejectionReason);
    }

    // --- RejectAsync ---

    [Fact]
    public void RejectCommand_WhenRejectionReasonEmpty_IsDisabled()
    {
        _sut.RejectionReason = string.Empty;

        Assert.False(_sut.RejectCommand.CanExecute(null));
    }

    [Fact]
    public void RejectCommand_WhenRejectionReasonNotEmpty_IsEnabled()
    {
        _sut.RejectionReason = "Damaged packaging";

        Assert.True(_sut.RejectCommand.CanExecute(null));
    }

    [Fact]
    public async Task RejectAsync_WhenSuccessful_CallsRejectReceptionAsync()
    {
        var reception = BuildReception("REC-001", 1, 5);
        SetupSession(reception);

        _sut.RejectionReason = "Damaged packaging";

        await _sut.RejectCommand.ExecuteAsync(null);

        _receptionServiceMock.Verify(x => x.RejectReceptionAsync(
            reception,
            "Damaged packaging"), Times.Once);
    }

    [Fact]
    public async Task RejectAsync_WhenSuccessful_ClearsReceptionSession()
    {
        var reception = BuildReception("REC-001", 1, 5);
        SetupSession(reception);

        _sut.RejectionReason = "Damaged packaging";

        await _sut.RejectCommand.ExecuteAsync(null);

        _receptionSessionServiceMock.Verify(x => x.ClearReception(), Times.Once);
    }

    [Fact]
    public async Task RejectAsync_WhenSuccessful_NavigatesToStartPageWithClearStack()
    {
        var reception = BuildReception("REC-001", 1, 5);
        SetupSession(reception);

        _sut.RejectionReason = "Damaged packaging";

        await _sut.RejectCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Reception.ReceptionStartPage),
            true), Times.Once);
    }

    [Fact]
    public async Task RejectAsync_WhenServiceThrows_SetsHasError()
    {
        var reception = BuildReception("REC-001", 1, 5);
        SetupSession(reception);

        _sut.RejectionReason = "Damaged packaging";

        _receptionServiceMock
            .Setup(x => x.RejectReceptionAsync(It.IsAny<ReceptionDto>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Service error"));

        await _sut.RejectCommand.ExecuteAsync(null);

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task RejectAsync_WhenComplete_IsBusyIsFalse()
    {
        var reception = BuildReception("REC-001", 1, 5);
        SetupSession(reception);

        _sut.RejectionReason = "Damaged packaging";

        await _sut.RejectCommand.ExecuteAsync(null);

        Assert.False(_sut.IsBusy);
    }

    // --- OnRejectionReasonChanged ---

    [Fact]
    public void OnRejectionReasonChanged_ClearsError()
    {
        _sut.RejectionReasonHasError = true;
        _sut.HasError = true;

        _sut.RejectionReason = "New reason";

        Assert.False(_sut.RejectionReasonHasError);
        Assert.False(_sut.HasError);
        Assert.Equal(string.Empty, _sut.ErrorMessage);
    }

    // --- GoBackAsync ---

    [Fact]
    public async Task GoBackAsync_NavigatesBack()
    {
        await _sut.GoBackCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateBackAsync(false), Times.Once);
    }

    // --- Helpers ---

    private static ReceptionDto BuildReception(string number, int lineCount, int quantityPerLine)
    {
        var reception = new ReceptionDto
        {
            ReceptionNumber = number,
            FlowType = ReceptionFlowType.Standard,
            Status = ReceptionStatus.New,
            Header = new ReceptionHeaderDto
            {
                Sender = "Supplier A",
                Recipient = "Warehouse 1",
            },
        };

        for (var i = 0; i < lineCount; i++)
        {
            reception.DetailLines.Add(new ReceptionDetailDto
            {
                Article = $"ART-00{i + 1}",
                Quantity = quantityPerLine,
                Location = "A-01-01",
            });
        }

        return reception;
    }

    private void SetupSession(ReceptionDto reception) =>
        _receptionSessionServiceMock
            .Setup(x => x.CurrentReception)
            .Returns(reception);
}
