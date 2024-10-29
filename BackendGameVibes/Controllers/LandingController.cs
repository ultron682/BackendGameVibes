using AutoMapper;
using BackendGameVibes.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendGameVibes.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class LandingController : ControllerBase {
        private readonly IGameService _gameService;
        private readonly IReviewService _reviewService;
        private readonly IForumThreadService _forumThreadService;
        private readonly IMapper _mapper;


        public LandingController(IGameService gameService, IReviewService reviewService, IForumThreadService forumThreadService, IMapper mapper) {
            _gameService = gameService;
            _reviewService = reviewService;
            _forumThreadService = forumThreadService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<object>> Get() {
            var landingPage = new {
                games = await _gameService.GetLandingGames(),
                reviews = await _reviewService.GetLandingReviewsAsync(),
                upcomingGames = await _gameService.GetUpcomingGames(),
                latestForumThreads = await _forumThreadService.GetLandingThreads()
            };

            return landingPage;
        }
    }
}
