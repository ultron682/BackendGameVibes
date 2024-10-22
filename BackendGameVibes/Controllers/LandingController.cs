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
        private readonly IMapper _mapper;


        public LandingController(IGameService gameService, IReviewService reviewService, IMapper mapper) {
            _gameService = gameService;
            _reviewService = reviewService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<object>> Get() {
            var landingPage = new {
                //games = _gameService.GetGames(),
                reviews = await _reviewService.GetLandingReviews()
            };

            return landingPage;
        }
    }
}
