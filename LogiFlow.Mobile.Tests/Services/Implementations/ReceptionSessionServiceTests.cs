using LogiFlow.Mobile.DTOs;
using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;
using Moq;

namespace LogiFlow.Mobile.Tests.Services.Implementations
{
    public class ReceptionSessionServiceTests
    {
        private readonly Mock<ILogService> _logServiceMock;
        private readonly ReceptionSessionService _service;

        public ReceptionSessionServiceTests()
        {
            _logServiceMock = new Mock<ILogService>();
            _service = new ReceptionSessionService(_logServiceMock.Object);
        }

        [Fact]
        public void StartReception_ShouldSetCurrentReception_AndLogInfo()
        {
            // Arrange
            var reception = new ReceptionDto { ReceptionNumber = "123", FlowType = "Test" };

            // Act
            _service.StartReception(reception);

            // Assert
            Assert.Equal(reception, _service.CurrentReception);
            Assert.True(_service.HasActiveReception);
            _logServiceMock.Verify(l => l.Info(
                It.Is<string>(s => s.Contains("Reception session started")),
                reception.ReceptionNumber, reception.FlowType), Times.Once);
        }

        [Fact]
        public void UpdateHeader_ShouldUpdateHeader_WhenReceptionExists()
        {
            // Arrange
            var reception = new ReceptionDto { ReceptionNumber = "123", FlowType = "Test" };
            var header = new ReceptionHeaderDto();
            _service.StartReception(reception);

            // Act
            _service.UpdateHeader(header);

            // Assert
            Assert.Equal(header, _service.CurrentReception.Header);
            _logServiceMock.Verify(l => l.Info(
                It.Is<string>(s => s.Contains("Reception header updated")),
                reception.ReceptionNumber), Times.Once);
        }

        [Fact]
        public void UpdateHeader_ShouldLogWarning_WhenNoReception()
        {
            // Act
            _service.UpdateHeader(new ReceptionHeaderDto());

            // Assert
            _logServiceMock.Verify(l => l.Warning(
                It.Is<string>(s => s.Contains("UpdateHeader called with no active reception."))), Times.Once);
        }

        [Fact]
        public void AddDetail_ShouldAddDetail_WhenReceptionExists()
        {
            // Arrange
            var reception = new ReceptionDto { ReceptionNumber = "123", FlowType = "Test", DetailLines = new List<ReceptionDetailDto>() };
            var detail = new ReceptionDetailDto { Article = "A1", Quantity = 5 };
            _service.StartReception(reception);

            // Act
            _service.AddDetail(detail);

            // Assert
            Assert.Contains(detail, _service.CurrentReception.DetailLines);
            _logServiceMock.Verify(l => l.Info(
                It.Is<string>(s => s.Contains("Detail added to reception")),
                reception.ReceptionNumber, detail.Article, detail.Quantity), Times.Once);
        }

        [Fact]
        public void AddDetail_ShouldLogWarning_WhenNoReception()
        {
            // Act
            _service.AddDetail(new ReceptionDetailDto());

            // Assert
            _logServiceMock.Verify(l => l.Warning(
                It.Is<string>(s => s.Contains("AddDetail called with no active reception."))), Times.Once);
        }

        [Fact]
        public void SetDetails_ShouldReplaceDetails_WhenReceptionExists()
        {
            // Arrange
            var reception = new ReceptionDto { ReceptionNumber = "123", FlowType = "Test", DetailLines = new List<ReceptionDetailDto>() };
            var details = new List<ReceptionDetailDto>
            {
                new ReceptionDetailDto { Article = "A1", Quantity = 1 },
                new ReceptionDetailDto { Article = "A2", Quantity = 2 }
            };
            _service.StartReception(reception);

            // Act
            _service.SetDetails(details);

            // Assert
            Assert.Equal(details, _service.CurrentReception.DetailLines);
            _logServiceMock.Verify(l => l.Info(
                It.Is<string>(s => s.Contains("Reception details replaced")),
                reception.ReceptionNumber, details.Count), Times.Once);
        }

        [Fact]
        public void SetDetails_ShouldLogWarning_WhenNoReception()
        {
            // Act
            _service.SetDetails(new List<ReceptionDetailDto>());

            // Assert
            _logServiceMock.Verify(l => l.Warning(
                It.Is<string>(s => s.Contains("SetDetails called with no active reception."))), Times.Once);
        }

        [Fact]
        public void ClearReception_ShouldClearCurrentReception_AndLogInfo()
        {
            // Arrange
            var reception = new ReceptionDto { ReceptionNumber = "123", FlowType = "Test" };
            _service.StartReception(reception);

            // Act
            _service.ClearReception();

            // Assert
            Assert.Null(_service.CurrentReception);
            Assert.False(_service.HasActiveReception);
            _logServiceMock.Verify(l => l.Info(
                It.Is<string>(s => s.Contains("Reception session cleared")),
                reception.ReceptionNumber), Times.Once);
        }

        [Fact]
        public void ClearReception_ShouldLogInfo_WhenNoReception()
        {
            // Act
            _service.ClearReception();

            // Assert
            _logServiceMock.Verify(l => l.Info(
                It.Is<string>(s => s.Contains("Reception session cleared")),
                "none"), Times.Once);
        }
    }
}
