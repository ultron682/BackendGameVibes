using Microsoft.AspNetCore.Mvc;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using BackendGameVibes.Models.Steam;
using Swashbuckle.AspNetCore.Annotations;
using BackendGameVibes.Models.Games;

namespace BackendGameVibes.Controllers;
[ApiController]
[Route("api/games")]
public class GameController : ControllerBase {
    private readonly IGameService _gameService;

    public GameController(IGameService gameService) {
        _gameService = gameService;
    }

    [SwaggerOperation("SortedBy: Rating = 1, ReleaseDate = 2, Name = 3, FollowedPlayers = 4")]
    [HttpGet]
    public async Task<ActionResult> GetFilteredGames([FromQuery] FiltersGamesDTO filtersGamesDTO, int pageNumber = 1, int resultSize = 10) {
        var filteredGames = await _gameService.GetFilteredGamesAsync(filtersGamesDTO, pageNumber, resultSize);

        if (filteredGames == null) {
            return NotFound("No games found with the given filters.");
        }

        return Ok(filteredGames);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetGame(int id) {
        var game = await _gameService.GetGameDetailsAsync(id);
        if (game == null) {
            return NotFound();
        }
        else {
            return Ok(game);
        }
    }

    [HttpGet("search-steam-game")]
    public ActionResult<SteamApp[]> FindSteamAppByName(string searchingName) {
        var steamApp = _gameService.FindSteamAppByName(searchingName);
        if (steamApp != null)
            return Ok(steamApp);
        else
            return BadRequest("steamApp == null. maybe json with 500k lines is downloading...");
    }

    [HttpGet("genres")]
    public async Task<ActionResult<object[]>> GetGenres() {
        var genres = await _gameService.GetGenresAsync();
        if (genres == null || genres.Length == 0)
            return NotFound("No Genres");

        return Ok(genres);
    }

    [HttpGet("platforms")]
    public async Task<ActionResult<object[]>> GetPlatforms() {
        var platforms = await _gameService.GetPlatformsAsync();
        if (platforms == null || platforms.Length == 0)
            return NotFound("No Platforms");

        return Ok(platforms);
    }

    [HttpPost]
    [Authorize(Roles = "admin,mod")]
    [SwaggerOperation("Require authorization admin or mod")]
    public async Task<ActionResult<Game>> CreateGame(int steamGameId = 292030) {
        (Game? game, bool isSuccess) = await _gameService.AddGameAsync(steamGameId);
        if (game == null && !isSuccess)
            return BadRequest("SteamGameData is null");
        else if (game != null && !isSuccess)
            return Conflict(game);
        else
            return Ok(game);
    }
}