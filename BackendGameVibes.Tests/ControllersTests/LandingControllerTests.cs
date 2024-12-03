using AutoMapper;
using BackendGameVibes.Controllers;
using BackendGameVibes.IServices.Forum;
using BackendGameVibes.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendGameVibes.Tests.Controllers {
    public class LandingControllerTests {
        private readonly LandingController _controller;
        private readonly Mock<IGameService> _gameServiceMock;
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly Mock<IForumThreadService> _forumThreadServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly MemoryCache _cache;

        public LandingControllerTests() {
            _gameServiceMock = new Mock<IGameService>();
            _reviewServiceMock = new Mock<IReviewService>();
            _forumThreadServiceMock = new Mock<IForumThreadService>();
            _mapperMock = new Mock<IMapper>();

            _cache = new MemoryCache(new MemoryCacheOptions());

            _controller = new LandingController(
                _cache,
                _gameServiceMock.Object,
                _reviewServiceMock.Object,
                _forumThreadServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Get_ReturnsCachedData_IfCacheExists() {
            // Arrange
            var cacheKey = "LandingPageData";
            var cachedData = new { games = new List<string> { "CachedGame" } };
            _cache.Set(cacheKey, cachedData);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<ActionResult<object>>(result);
            Assert.Equal(cachedData, okResult.Value);
        }

        [Fact]
        public async Task Get_FetchesDataAndCaches_WhenCacheIsEmpty() {
            // Arrange
            var games = new object[] { "Game1", "Game2" };
            var reviews = new object[] { "Review1", "Review2" };
            var upcomingGames = new object[] { "Upcoming1", "Upcoming2" };
            var threads = new object[] { "Thread1", "Thread2" };

            _gameServiceMock.Setup(s => s.GetLandingGamesAsync()).ReturnsAsync(games);
            _reviewServiceMock.Setup(s => s.GetLandingReviewsAsync()).ReturnsAsync(reviews);
            _gameServiceMock.Setup(s => s.GetUpcomingGamesAsync()).ReturnsAsync(upcomingGames);
            _forumThreadServiceMock.Setup(s => s.GetThreadsLandingAsync()).ReturnsAsync(threads);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<ActionResult<object>>(result);
            dynamic landingPageResult = okResult.Value!;
            Assert.NotNull(landingPageResult);
        }
    }
}
