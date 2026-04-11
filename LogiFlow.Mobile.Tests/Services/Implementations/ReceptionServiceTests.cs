using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services.Implementations
{
    public class ReceptionServiceTests
    {
        private readonly Mock<ILogService> _logServiceMock;
        private readonly ReceptionService _service;

        public ReceptionServiceTests()
        {
            _logServiceMock = new Mock<ILogService>();
            _service = new ReceptionService(_logServiceMock.Object);
        }

        [Theory]
        [InlineData("REC-001", ReceptionFlowType.Standard, "Supplier A", "Warehouse 1", "DN-2024-001")]
        [InlineData("rec-002", ReceptionFlowType.TestSample, "Lab B", "QA Zone", "DN-2024-002")]
        public async Task LoadReceptionAsync_ShouldReturnReception_WhenReceptionExists(
            string receptionNumber, string expectedFlowType, string expectedSender, string expectedRecipient, string expectedDeliveryNote)
        {
            // Act
            var result = await _service.LoadReceptionAsync(receptionNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(receptionNumber.ToUpperInvariant(), result.ReceptionNumber);
            Assert.Equal(expectedFlowType, result.FlowType);
            Assert.Equal(ReceptionStatus.New, result.Status);
            Assert.Equal(expectedSender, result.Header.Sender);
            Assert.Equal(expectedRecipient, result.Header.Recipient);
            Assert.Equal(expectedDeliveryNote, result.Header.DeliveryNote);
            _logServiceMock.Verify(l => l.Info(
                It.Is<string>(s => s.Contains("Reception loaded")),
                receptionNumber, expectedFlowType), Times.Once);
        }

        [Fact]
        public async Task LoadReceptionAsync_ShouldReturnNull_AndLogWarning_WhenReceptionDoesNotExist()
        {
            // Act
            var result = await _service.LoadReceptionAsync("INVALID-999");

            // Assert
            Assert.Null(result);
            _logServiceMock.Verify(l => l.Warning(
                It.Is<string>(s => s.Contains("Reception not found")),
                "INVALID-999"), Times.Once);
        }

        [Fact]
        public async Task SaveReceptionAsync_ShouldStoreReception_AndLogInfo()
        {
            // Arrange
            var reception = new ReceptionDto
            {
                ReceptionNumber = "REC-001",
                Status = ReceptionStatus.New
            };

            // Act
            await _service.SaveReceptionAsync(reception);

            // Assert
            _logServiceMock.Verify(l => l.Info(
                It.Is<string>(s => s.Contains("Reception saved locally")),
                reception.ReceptionNumber, reception.Status), Times.Once);
        }

        [Fact]
        public async Task ConfirmReceptionAsync_ShouldSetStatusToConfirmed_AndReturnItems_AndLog()
        {
            // Arrange
            var reception = new ReceptionDto
            {
                ReceptionNumber = "REC-001",
                Status = ReceptionStatus.New,
                DetailLines = new List<ReceptionDetailDto>
                {
                    new ReceptionDetailDto { Article = "A1", ArticleDescription = "Desc1", Quantity = 2, Location = "L1" },
                    new ReceptionDetailDto { Article = "A2", ArticleDescription = "Desc2", Quantity = 3, Location = "L2" }
                }
            };

            // Act
            var items = await _service.ConfirmReceptionAsync(reception);

            // Assert
            Assert.Equal(ReceptionStatus.Confirmed, reception.Status);
            Assert.Equal(2, items.Count);
            Assert.All(items, i => Assert.Equal("GENERATED", i.Status));
            Assert.Equal("A1", items[0].Article);
            Assert.Equal("A2", items[1].Article);
            _logServiceMock.Verify(l => l.OperationStart("ConfirmReception", reception.ReceptionNumber, null), Times.Once);
            _logServiceMock.Verify(l => l.OperationSuccess(
                "ConfirmReception", reception.ReceptionNumber, It.Is<string>(s => s.Contains("ItemsGenerated=2"))), Times.Once);
        }

        [Fact]
        public async Task RejectReceptionAsync_ShouldSetStatusToRejected_AndLogInfo()
        {
            // Arrange
            var reception = new ReceptionDto
            {
                ReceptionNumber = "REC-001",
                Status = ReceptionStatus.New
            };
            var reason = "Damaged goods";

            // Act
            await _service.RejectReceptionAsync(reception, reason);

            // Assert
            Assert.Equal(ReceptionStatus.Rejected, reception.Status);
            _logServiceMock.Verify(l => l.Info(
                It.Is<string>(s => s.Contains("Reception rejected")),
                reception.ReceptionNumber, reason), Times.Once);
        }

        [Fact]
        public void IsChecklistRequired_ShouldReturnTrue_WhenFlowTypeIsTestSample()
        {
            // Arrange
            var reception = new ReceptionDto
            {
                FlowType = ReceptionFlowType.TestSample,
                DetailLines = new List<ReceptionDetailDto>()
            };

            // Act
            var result = _service.IsChecklistRequired(reception);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsChecklistRequired_ShouldReturnTrue_WhenAnyDetailIsVehicle()
        {
            // Arrange
            var reception = new ReceptionDto
            {
                FlowType = ReceptionFlowType.Standard,
                DetailLines = new List<ReceptionDetailDto>
                {
                    new ReceptionDetailDto { Article = "VEHICLE" }
                }
            };

            // Act
            var result = _service.IsChecklistRequired(reception);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsChecklistRequired_ShouldReturnFalse_WhenNoChecklistConditionMet()
        {
            // Arrange
            var reception = new ReceptionDto
            {
                FlowType = ReceptionFlowType.Standard,
                DetailLines = new List<ReceptionDetailDto>
                {
                    new ReceptionDetailDto { Article = "BOX" }
                }
            };

            // Act
            var result = _service.IsChecklistRequired(reception);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsChecklistRequired_ShouldBeCaseInsensitive_ForVehicle()
        {
            // Arrange
            var reception = new ReceptionDto
            {
                FlowType = ReceptionFlowType.Standard,
                DetailLines = new List<ReceptionDetailDto>
                {
                    new ReceptionDetailDto { Article = "vehicle" }
                }
            };

            // Act
            var result = _service.IsChecklistRequired(reception);

            // Assert
            Assert.True(result);
        }
    }
}
