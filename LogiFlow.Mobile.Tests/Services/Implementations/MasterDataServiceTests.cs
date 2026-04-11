using LogiFlow.Mobile.Services.Implementations;
using LogiFlow.Mobile.Services.Interfaces;

namespace LogiFlow.Mobile.Tests.Services.Implementations
{
    public class MasterDataServiceTests
    {
        private readonly IMasterDataService _service;

        public MasterDataServiceTests()
        {
            _service = new MasterDataService();
        }

        [Fact]
        public async Task GetArticlesAsync_ShouldReturnAllArticles()
        {
            // Act
            var articles = await _service.GetArticlesAsync();

            // Assert
            Assert.NotNull(articles);
            Assert.Contains("ART-001", articles);
            Assert.Contains("VEHICLE", articles);
            Assert.Equal(6, articles.Count);
        }

        [Fact]
        public async Task GetLocationsAsync_ShouldReturnAllLocations()
        {
            // Act
            var locations = await _service.GetLocationsAsync();

            // Assert
            Assert.NotNull(locations);
            Assert.Contains("A-01-01", locations);
            Assert.Contains("QA-ZONE-01", locations);
            Assert.Equal(6, locations.Count);
        }

        [Fact]
        public async Task GetSendersAsync_ShouldReturnAllSenders()
        {
            // Act
            var senders = await _service.GetSendersAsync();

            // Assert
            Assert.NotNull(senders);
            Assert.Contains("Supplier A", senders);
            Assert.Contains("Lab B", senders);
            Assert.Equal(4, senders.Count);
        }

        [Fact]
        public async Task GetRecipientsAsync_ShouldReturnAllRecipients()
        {
            // Act
            var recipients = await _service.GetRecipientsAsync();

            // Assert
            Assert.NotNull(recipients);
            Assert.Contains("Warehouse 1", recipients);
            Assert.Contains("Production", recipients);
            Assert.Equal(4, recipients.Count);
        }

        [Theory]
        [InlineData("A-01-01", true)]
        [InlineData("QA-ZONE-01", true)]
        [InlineData("a-01-01", true)] // Case-insensitive
        [InlineData("invalid-location", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public async Task IsValidLocationAsync_ShouldReturnExpectedResult(string locationCode, bool expected)
        {
            // Act
            var result = await _service.IsValidLocationAsync(locationCode);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("ART-001", true)]
        [InlineData("VEHICLE", true)]
        [InlineData("art-001", true)] // Case-insensitive
        [InlineData("unknown-article", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public async Task IsValidArticleAsync_ShouldReturnExpectedResult(string articleCode, bool expected)
        {
            // Act
            var result = await _service.IsValidArticleAsync(articleCode);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
