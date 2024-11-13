using AutoMapper;
using BackendGameVibes.IServices;
using BackendGameVibes.IServices.Forum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BackendGameVibes.Controllers {
    [Route("api/landing")]
    [ApiController]
    public class LandingController : ControllerBase {
        private readonly IGameService _gameService;
        private readonly IReviewService _reviewService;
        private readonly IForumThreadService _forumThreadService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;


        public LandingController(IMemoryCache cache, IGameService gameService, IReviewService reviewService, IForumThreadService forumThreadService, IMapper mapper) {
            _cache = cache;
            _gameService = gameService;
            _reviewService = reviewService;
            _forumThreadService = forumThreadService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<object>> Get() {
            var cacheKey = "LandingPageData";

            if (!_cache.TryGetValue(cacheKey, out object? landingPage)) {
                landingPage = new {
                    games = await _gameService.GetLandingGames(),
                    reviews = await _reviewService.GetLandingReviewsAsync(),
                    upcomingGames = await _gameService.GetUpcomingGames(),
                    latestForumThreads = await _forumThreadService.GetThreadsLandingAsync()
                };

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(cacheKey, landingPage, cacheOptions);
            }

            return landingPage!;
        }
    }
}
