using Microsoft.AspNetCore.Mvc;
using BackendGameVibes.Services;
using System.Threading.Tasks;
using BackendGameVibes.Models;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using BackendGameVibes.Models.Steam;

namespace BackendGameVibes.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService) {
            _gameService = gameService;
        }

        [HttpGet("steamIDs")]
        public ActionResult<SteamApp[]> GetAllSteamIdsGames() {
            var steamGames = _gameService.GetAllSteamIdsGames();
            if (steamGames != null)
                return Ok(steamGames);
            else
                return NotFound("GetAllSteamIdsGames unsuccessful");
        }

        [HttpGet("search")]
        public ActionResult<SteamApp[]> FindSteamAppByName(string searchingName) {
            var steamApp = _gameService.FindSteamAppByName(searchingName);
            if (steamApp != null)
                return Ok(steamApp);
            else
                return BadRequest("steamApp == null. maybe json with 500k lines is downloading...");
        }

        [HttpGet]
        public async Task<ActionResult<object[]>> GetGames() {
            var games = await _gameService.GetGames();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetGame(int id) {
            var game = await _gameService.GetGame(id);
            if (game == null) {
                return NotFound();
            }
            else {
                return Ok(game);
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,mod")]
        public async Task<ActionResult<Game>> CreateGame(int steamGameId = 292030) {
            var game = await _gameService.CreateGame(steamGameId);
            if (game == null)
                return BadRequest("SteamGameData is null");

            return Ok(game);
        }

        [HttpGet("genres")]
        public async Task<ActionResult<object[]>> GetGenres() {
            var genres = await _gameService.GetGenres();
            if (genres == null || genres.Length == 0)
                return NotFound("No Genres");

            return Ok(genres);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<object>>> GetFilteredGames([FromQuery] FiltersGamesDTO filtersGamesDTO) {
            var filteredGames = await _gameService.GetFilteredGames(filtersGamesDTO);

            if (filteredGames == null || !filteredGames.Any()) {
                return NotFound("No games found with the given filters.");
            }

            return Ok(filteredGames);
        }
    }
}