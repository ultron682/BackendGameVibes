using Microsoft.AspNetCore.Mvc;
using BackendGameVibes.Services;
using BackendGameVibes.SteamApiModels;
using System.Threading.Tasks;
using BackendGameVibes.Models;
using BackendGameVibes.IServices;

namespace BackendGameVibes.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService) {
            _gameService = gameService;
        }

        [HttpGet("steamIDs")]
        public async Task<ActionResult<SteamApp[]>> GetAllSteamIdsGames() {
            var steamGames = await _gameService.GetAllSteamIdsGames();
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
        public async Task<ActionResult<Game>> CreateGame(int steamGameId = 292030) {
            var game = await _gameService.CreateGame(steamGameId);
            if (game == null)
                return BadRequest("SteamGameData is null");

            return Ok(game);
        }
    }
}