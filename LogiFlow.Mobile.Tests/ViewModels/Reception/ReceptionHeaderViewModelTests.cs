using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Reception;
using Moq;

namespace LogiFlow.Mobile.Tests.ViewModels.Reception;

/// <summary>
/// Unit tests for <see cref="ReceptionHeaderViewModel"/>.
/// Covers loading, validation, navigation and state management.
/// </summary>
public class ReceptionHeaderViewModelTests
{
    private readonly Mock<IReceptionSessionService> _receptionSessionServiceMock;
    private readonly Mock<IMasterDataService> _masterDataServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<IErrorHandlerService> _errorHandlerServiceMock;
    private readonly Mock<IChatDialogService> _chatDialogService;

    private readonly ReceptionHeaderViewModel _sut;

    public ReceptionHeaderViewModelTests()
    {
        _receptionSessionServiceMock = new Mock<IReceptionSessionService>();
        _masterDataServiceMock = new Mock<IMasterDataService>();
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

        _masterDataServiceMock
            .Setup(x => x.GetSendersAsync())
            .ReturnsAsync(["Supplier A", "Supplier B", "Lab B"]);

        _masterDataServiceMock
            .Setup(x => x.GetRecipientsAsync())
            .ReturnsAsync(["Warehouse 1", "Warehouse 2", "QA Zone"]);

        _sut = new ReceptionHeaderViewModel(
            _receptionSessionServiceMock.Object,
            _masterDataServiceMock.Object,
            _navigationServiceMock.Object,
            _localizationServiceMock.Object,
            _logServiceMock.Object,
            _errorHandlerServiceMock.Object,
             _chatDialogService.Object);
    }

    // --- Initial state ---

    [Fact]
    public void InitialState_HasErrorIsFalse()
    {
        Assert.False(_sut.HasError);
    }

    [Fact]
    public void InitialState_SenderIsEmpty()
    {
        Assert.Equal(string.Empty, _sut.Sender);
    }

    [Fact]
    public void InitialState_RecipientIsEmpty()
    {
        Assert.Equal(string.Empty, _sut.Recipient);
    }

    // --- LoadAsync ---

    [Fact]
    public async Task LoadAsync_WhenNoActiveSession_SetsHasError()
    {
        _receptionSessionServiceMock
            .Setup(x => x.CurrentReception)
            .Returns((ReceptionDto?)null);

        await _sut.LoadAsync();

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task LoadAsync_WhenNoActiveSession_SetsErrorMessage()
    {
        _receptionSessionServiceMock
            .Setup(x => x.CurrentReception)
            .Returns((ReceptionDto?)null);

        await _sut.LoadAsync();

        Assert.NotEmpty(_sut.ErrorMessage);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionActive_SetsReceptionNumber()
    {
        var reception = BuildReception("REC-001", "Supplier A", "Warehouse 1");
        SetupSession(reception);

        await _sut.LoadAsync();

        Assert.Equal("REC-001", _sut.ReceptionNumber);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionActive_SetsSender()
    {
        var reception = BuildReception("REC-001", "Supplier A", "Warehouse 1");
        SetupSession(reception);

        await _sut.LoadAsync();

        Assert.Equal("Supplier A", _sut.Sender);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionActive_SetsRecipient()
    {
        var reception = BuildReception("REC-001", "Supplier A", "Warehouse 1");
        SetupSession(reception);

        await _sut.LoadAsync();

        Assert.Equal("Warehouse 1", _sut.Recipient);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionActive_SetsFlowType()
    {
        var reception = BuildReception("REC-001", "Supplier A", "Warehouse 1");
        SetupSession(reception);

        await _sut.LoadAsync();

        Assert.Equal(ReceptionFlowType.Standard, _sut.FlowType);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionActive_LoadsAvailableSenders()
    {
        var reception = BuildReception("REC-001", "Supplier A", "Warehouse 1");
        SetupSession(reception);

        await _sut.LoadAsync();

        Assert.NotEmpty(_sut.AvailableSenders);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionActive_LoadsAvailableRecipients()
    {
        var reception = BuildReception("REC-001", "Supplier A", "Warehouse 1");
        SetupSession(reception);

        await _sut.LoadAsync();

        Assert.NotEmpty(_sut.AvailableRecipients);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionActive_LoadsSendersBeforeAssigningSender()
    {
        // Verifies that senders are loaded before the sender value is set
        // so the Picker can find the value in the list
        var reception = BuildReception("REC-001", "Supplier A", "Warehouse 1");
        SetupSession(reception);

        await _sut.LoadAsync();

        Assert.Contains("Supplier A", _sut.AvailableSenders);
        Assert.Equal("Supplier A", _sut.Sender);
    }

    [Fact]
    public async Task LoadAsync_WhenComplete_IsBusyIsFalse()
    {
        var reception = BuildReception("REC-001", "Supplier A", "Warehouse 1");
        SetupSession(reception);

        await _sut.LoadAsync();

        Assert.False(_sut.IsBusy);
    }

    [Fact]
    public async Task LoadAsync_WhenServiceThrows_SetsHasError()
    {
        var reception = BuildReception("REC-001", "Supplier A", "Warehouse 1");
        SetupSession(reception);

        _masterDataServiceMock
            .Setup(x => x.GetSendersAsync())
            .ThrowsAsync(new Exception("Service error"));

        await _sut.LoadAsync();

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task LoadAsync_WhenCalledTwice_ClearsStateBeforeReloading()
    {
        var reception = BuildReception("REC-001", "Supplier A", "Warehouse 1");
        SetupSession(reception);

        await _sut.LoadAsync();

        var reception2 = BuildReception("REC-002", "Lab B", "QA Zone");
        SetupSession(reception2);

        await _sut.LoadAsync();

        Assert.Equal("REC-002", _sut.ReceptionNumber);
        Assert.Equal("Lab B", _sut.Sender);
        Assert.Equal("QA Zone", _sut.Recipient);
    }

    // --- ContinueAsync ---

    [Fact]
    public async Task ContinueAsync_WhenSenderEmpty_SetsHasError()
    {
        _sut.Sender = string.Empty;
        _sut.Recipient = "Warehouse 1";

        await _sut.ContinueCommand.ExecuteAsync(null);

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task ContinueAsync_WhenRecipientEmpty_SetsHasError()
    {
        _sut.Sender = "Supplier A";
        _sut.Recipient = string.Empty;

        await _sut.ContinueCommand.ExecuteAsync(null);

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task ContinueAsync_WhenSenderEmpty_DoesNotNavigate()
    {
        _sut.Sender = string.Empty;
        _sut.Recipient = "Warehouse 1";

        await _sut.ContinueCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            It.IsAny<string>(),
            It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task ContinueAsync_WhenValid_UpdatesSessionHeader()
    {
        _sut.Sender = "Supplier A";
        _sut.Recipient = "Warehouse 1";
        _sut.DeliveryNote = "DN-001";
        _sut.Observations = "Test observations";

        await _sut.ContinueCommand.ExecuteAsync(null);

        _receptionSessionServiceMock.Verify(x => x.UpdateHeader(
            It.Is<ReceptionHeaderDto>(h =>
                h.Sender == "Supplier A" &&
                h.Recipient == "Warehouse 1" &&
                h.DeliveryNote == "DN-001")),
            Times.Once);
    }

    [Fact]
    public async Task ContinueAsync_WhenValid_NavigatesToDetailPage()
    {
        _sut.Sender = "Supplier A";
        _sut.Recipient = "Warehouse 1";

        await _sut.ContinueCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Reception.ReceptionDetailPage),
            false), Times.Once);
    }

    [Fact]
    public async Task ContinueAsync_WhenNavigationThrows_SetsHasError()
    {
        _sut.Sender = "Supplier A";
        _sut.Recipient = "Warehouse 1";

        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new Exception("Navigation error"));

        await _sut.ContinueCommand.ExecuteAsync(null);

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task ContinueAsync_WhenComplete_IsBusyIsFalse()
    {
        _sut.Sender = "Supplier A";
        _sut.Recipient = "Warehouse 1";

        await _sut.ContinueCommand.ExecuteAsync(null);

        Assert.False(_sut.IsBusy);
    }

    // --- GoBackAsync ---

    [Fact]
    public async Task GoBackAsync_NavigatesBack()
    {
        await _sut.GoBackCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateBackAsync(false), Times.Once);
    }

    // --- Helpers ---

    private static ReceptionDto BuildReception(string number, string sender, string recipient) =>
        new()
        {
            ReceptionNumber = number,
            FlowType = ReceptionFlowType.Standard,
            Status = ReceptionStatus.New,
            Header = new ReceptionHeaderDto
            {
                Sender = sender,
                Recipient = recipient,
                DeliveryNote = "DN-001",
                Observations = string.Empty,
                ExpectedDate = DateTime.Today,
            },
        };

    private void SetupSession(ReceptionDto reception) =>
        _receptionSessionServiceMock
            .Setup(x => x.CurrentReception)
            .Returns(reception);
}
