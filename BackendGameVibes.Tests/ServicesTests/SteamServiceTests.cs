using BackendGameVibes.Models.Steam;
using BackendGameVibes.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendGameVibes.Tests.Services {
    public class SteamServiceTests {
        private readonly Mock<HttpClient> _httpClientMock;
        private readonly SteamService _steamService;

        public SteamServiceTests() {
            _httpClientMock = new Mock<HttpClient>();
            _steamService = new SteamService(_httpClientMock.Object);
        }

        [Fact]
        public void FindSteamAppByName_ReturnsMatchingApps() {
            // Arrange
            var searchResult = new[] {
                new SteamApp { Name = "Test Game" }
            };
            _steamService.steamGames = searchResult;

            // Act
            var result = _steamService.FindSteamApp("Test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Game", result[0].Name);
        }
    }
}
