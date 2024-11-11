using Microsoft.AspNetCore.Mvc;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using BackendGameVibes.Models.Steam;
using Swashbuckle.AspNetCore.Annotations;
using BackendGameVibes.Models.Games;
using System.Security.Claims;

namespace BackendGameVibes.Controllers;
[ApiController]
[Route("api/game")]
public class GameController : ControllerBase {
    private readonly IGameService _gameService;

    public GameController(IGameService gameService) {
        _gameService = gameService;
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
    public async Task<ActionResult> GetGames(int pageNumber = 1, int resultSize = 10) {
        var games = await _gameService.GetGames(pageNumber, resultSize);
        return Ok(games);
    }

    [HttpGet("filter")]
    public async Task<ActionResult> GetFilteredGames([FromQuery] FiltersGamesDTO filtersGamesDTO, int pageNumber = 1, int resultSize = 10) {
        var filteredGames = await _gameService.GetFilteredGames(filtersGamesDTO, pageNumber, resultSize);

        if (filteredGames == null) {
            return NotFound("No games found with the given filters.");
        }

        return Ok(filteredGames);
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

    [HttpGet("{id:int}/reviews")]
    public async Task<ActionResult> GetGameReviews(int id, int pageNumber = 1, int resultSize = 10) {
        var gameReviews = await _gameService.GetGameReviews(id, pageNumber, resultSize);
        if (gameReviews == null) {
            return NotFound();
        }
        else {
            return Ok(gameReviews);
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin,mod")]
    [SwaggerOperation("Require authorization admin or mod")]
    public async Task<ActionResult<Game>> CreateGame(int steamGameId = 292030) {
        (Game? game, bool isSuccess) = await _gameService.CreateGame(steamGameId);
        if (game == null && !isSuccess)
            return BadRequest("SteamGameData is null");
        else if (game != null && !isSuccess)
            return Conflict(game);
        else
            return Ok(game);
    }

    [HttpGet("genres")]
    public async Task<ActionResult<object[]>> GetGenres() {
        var genres = await _gameService.GetGenres();
        if (genres == null || genres.Length == 0)
            return NotFound("No Genres");

        return Ok(genres);
    }
}