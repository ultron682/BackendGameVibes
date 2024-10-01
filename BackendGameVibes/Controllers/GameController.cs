using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Models;
using BackendGameVibes.Data;
using BackendGameVibes.Services;

namespace BackendGameVibes.Controllers {
    //Game, Platform, and Genre
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase {
        private readonly ApplicationDbContext _context;
        private readonly SteamService _steamService;




        public GameController(ApplicationDbContext context, SteamService steamService) {
            _context = context;
            _steamService = steamService;
        }

        [HttpGet("steamIDs")]
        public async Task<ActionResult<SteamApp[]>> GetAllSteamIdsGames() {
            // ONLY EXAMPLE
            if (_steamService.steamGames != null)
                return Ok(_steamService.steamGames[29000..30000] ); // example only small range
            else
                return NotFound("GetAllSteamIdsGames unsuccessful");
        }

        [HttpGet("search")]
        public async Task<ActionResult<SteamApp[]>> FindSteamAppByName(string searchingName) {
            var steamApp = _steamService.FindSteamApp(searchingName);
            if (steamApp != null)
                return Ok(steamApp);
            else
                return BadRequest("steamApp == null");
        }

        // Get all games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames() {
            return await _context.Games
                .Include(g => g.Platform)
                .Include(g => g.Genres)
                .Include(g => g.GameImages)
                .Include(g => g.SystemRequirements)
                .ToListAsync();
        }

        // Get a single game by ID
        [HttpGet("{id}")]
        public async Task<ActionResult> GetGame(int id) {
            var game = await _context.Games
                .Where(g => g.Id == id)
                .Include(g => g.Platform)
                .Include(g => g.Genres)
                .Include(g => g.GameImages)
                .Include(g => g.SystemRequirements)
                .Select(g => new {
                    g.Id,
                    g.Title,
                    g.Description,
                    Platform = new { g.Platform.Id, g.Platform.Name },
                    g.ReleaseDate,
                    g.SteamId,
                    Genres = g.Genres.Select(g => new { g.Id, g.Name }).ToArray(),
                    GameImages = g.GameImages.Select(image => new { image.ImagePath }),
                    //SystemRequirements = g.SystemRequirements.Select()
                })
                .FirstOrDefaultAsync();



            if (game == null) {
                return NotFound();
            }
            else {
                var data = await _steamService.GetInfoGame(game.SteamId);

                Console.WriteLine(data.ToString());
                return Ok(game);
            }
        }

        /*
{
  "title": "My New Game",
  "description": "An amazing new game description",
  "releaseDate": "2024-10-01",
  "platformId": 1,
  "genres": [
    { "id": 1 },
    { "id": 2 }
  ],
  "steamid": 3111230,
  "platform": {"id": 2},
  "gameImages": [],
  "systemRequirements": []
}
         */

        // Create a new game
        [HttpPost]
        public async Task<ActionResult<Game>> CreateGame(Game game) {
            var genreIds = game.Genres.Select(g => g.Id).ToList();
            var existingGenres = await _context.Genres.Where(g => genreIds.Contains(g.Id)).ToListAsync();

            game.Genres = existingGenres;

            var platformId = game.Platform.Id;
            var existingPlatform = await _context.Platforms.Where(g => g.Id == platformId).FirstOrDefaultAsync();

            game.Platform = existingPlatform;

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
        }

        // Update a game
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGame(int id, Game game) {
            if (id != game.Id) {
                return BadRequest();
            }

            _context.Entry(game).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!_context.Games.Any(g => g.Id == id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // Delete a game
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id) {
            var game = await _context.Games.FindAsync(id);
            if (game == null) {
                return NotFound();
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
