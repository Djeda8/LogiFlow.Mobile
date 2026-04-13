using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Interfaces;
using LogiFlow.Mobile.ViewModels.Reception;
using Moq;

namespace LogiFlow.Mobile.Tests.ViewModels.Reception;

/// <summary>
/// Unit tests for <see cref="ReceptionItemsViewModel"/>.
/// Covers item loading, totals calculation, error handling and finish navigation.
/// </summary>
public class ReceptionItemsViewModelTests
{
    private readonly Mock<IReceptionService> _receptionServiceMock;
    private readonly Mock<IReceptionSessionService> _receptionSessionServiceMock;
    private readonly Mock<INavigationService> _navigationServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly Mock<IErrorHandlerService> _errorHandlerServiceMock;
    private readonly Mock<IChatDialogService> _chatDialogService;

    private readonly ReceptionItemsViewModel _sut;

    public ReceptionItemsViewModelTests()
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

        _sut = new ReceptionItemsViewModel(
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
    public void InitialState_GeneratedItemsIsEmpty()
    {
        Assert.Empty(_sut.GeneratedItems);
    }

    [Fact]
    public void InitialState_TotalItemsIsZero()
    {
        Assert.Equal(0, _sut.TotalItems);
    }

    [Fact]
    public void InitialState_TotalQuantityIsZero()
    {
        Assert.Equal(0, _sut.TotalQuantity);
    }

    [Fact]
    public void InitialState_HasErrorIsFalse()
    {
        Assert.False(_sut.HasError);
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
    public async Task LoadAsync_WhenNoActiveSession_IsBusyIsFalse()
    {
        _receptionSessionServiceMock
            .Setup(x => x.CurrentReception)
            .Returns((ReceptionDto?)null);

        await _sut.LoadAsync();

        Assert.False(_sut.IsBusy);
    }

    [Fact]
    public async Task LoadAsync_WhenSessionActive_SetsReceptionNumber()
    {
        var reception = BuildReception("REC-001");
        SetupSession(reception);
        SetupConfirmReception(reception, BuildItems(2, 5));

        await _sut.LoadAsync();

        Assert.Equal("REC-001", _sut.ReceptionNumber);
    }

    [Fact]
    public async Task LoadAsync_WhenItemsGenerated_PopulatesGeneratedItems()
    {
        var reception = BuildReception("REC-001");
        var items = BuildItems(3, 5);
        SetupSession(reception);
        SetupConfirmReception(reception, items);

        await _sut.LoadAsync();

        Assert.Equal(3, _sut.GeneratedItems.Count);
    }

    [Fact]
    public async Task LoadAsync_WhenItemsGenerated_SetsTotalItems()
    {
        var reception = BuildReception("REC-001");
        var items = BuildItems(3, 5);
        SetupSession(reception);
        SetupConfirmReception(reception, items);

        await _sut.LoadAsync();

        Assert.Equal(3, _sut.TotalItems);
    }

    [Fact]
    public async Task LoadAsync_WhenItemsGenerated_SetsTotalQuantity()
    {
        var reception = BuildReception("REC-001");
        var items = BuildItems(3, 5); // 3 items x 5 quantity = 15
        SetupSession(reception);
        SetupConfirmReception(reception, items);

        await _sut.LoadAsync();

        Assert.Equal(15, _sut.TotalQuantity);
    }

    [Fact]
    public async Task LoadAsync_WhenComplete_IsBusyIsFalse()
    {
        var reception = BuildReception("REC-001");
        SetupSession(reception);
        SetupConfirmReception(reception, BuildItems(1, 3));

        await _sut.LoadAsync();

        Assert.False(_sut.IsBusy);
    }

    [Fact]
    public async Task LoadAsync_WhenServiceThrows_SetsHasError()
    {
        var reception = BuildReception("REC-001");
        SetupSession(reception);

        _receptionServiceMock
            .Setup(x => x.ConfirmReceptionAsync(reception))
            .ThrowsAsync(new Exception("Service error"));

        await _sut.LoadAsync();

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task LoadAsync_WhenServiceThrows_SetsErrorMessage()
    {
        var reception = BuildReception("REC-001");
        SetupSession(reception);

        _receptionServiceMock
            .Setup(x => x.ConfirmReceptionAsync(reception))
            .ThrowsAsync(new Exception("Service error"));

        await _sut.LoadAsync();

        Assert.NotEmpty(_sut.ErrorMessage);
    }

    [Fact]
    public async Task LoadAsync_WhenCalledTwice_ClearsAndRepopulatesItems()
    {
        var reception = BuildReception("REC-001");
        SetupSession(reception);
        SetupConfirmReception(reception, BuildItems(2, 3));

        await _sut.LoadAsync();
        await _sut.LoadAsync();

        Assert.Equal(2, _sut.GeneratedItems.Count);
    }

    // --- FinishAsync ---

    [Fact]
    public async Task FinishAsync_ClearsReceptionSession()
    {
        await _sut.FinishCommand.ExecuteAsync(null);

        _receptionSessionServiceMock.Verify(x => x.ClearReception(), Times.Once);
    }

    [Fact]
    public async Task FinishAsync_NavigatesToMenuWithClearStack()
    {
        await _sut.FinishCommand.ExecuteAsync(null);

        _navigationServiceMock.Verify(x => x.NavigateToAsync(
            nameof(Views.Menu.MenuPage),
            true), Times.Once);
    }

    [Fact]
    public async Task FinishAsync_WhenNavigationThrows_SetsHasError()
    {
        _navigationServiceMock
            .Setup(x => x.NavigateToAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new Exception("Navigation error"));

        await _sut.FinishCommand.ExecuteAsync(null);

        Assert.True(_sut.HasError);
    }

    [Fact]
    public async Task FinishAsync_WhenComplete_IsBusyIsFalse()
    {
        await _sut.FinishCommand.ExecuteAsync(null);

        Assert.False(_sut.IsBusy);
    }

    // --- Helpers ---

    private static ReceptionDto BuildReception(string number) =>
        new()
        {
            ReceptionNumber = number,
            FlowType = ReceptionFlowType.Standard,
            Status = ReceptionStatus.Confirmed,
            Header = new ReceptionHeaderDto
            {
                Sender = "Supplier A",
                Recipient = "Warehouse 1",
            },
            DetailLines =
            [
                new ReceptionDetailDto
                {
                    Article = "ART-001",
                    Quantity = 5,
                    Location = "A-01-01",
                },
            ],
        };

    private static List<ItemDto> BuildItems(int count, int quantityEach) =>
        Enumerable.Range(1, count).Select(i => new ItemDto
        {
            ItemCode = $"ITEM-{i:D3}",
            Article = "ART-001",
            Quantity = quantityEach,
            Location = "A-01-01",
            Status = "GENERATED",
        }).ToList();

    private void SetupSession(ReceptionDto reception) =>
        _receptionSessionServiceMock
            .Setup(x => x.CurrentReception)
            .Returns(reception);

    private void SetupConfirmReception(ReceptionDto reception, List<ItemDto> items) =>
        _receptionServiceMock
            .Setup(x => x.ConfirmReceptionAsync(reception))
            .ReturnsAsync(items);
}
