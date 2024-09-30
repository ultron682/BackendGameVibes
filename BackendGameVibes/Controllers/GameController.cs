using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Models;
using BackendGameVibes.Data;

namespace BackendGameVibes.Controllers {
    //Game, Platform, and Genre
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public GameController(ApplicationDbContext context) {
            _context = context;
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
        public async Task<ActionResult<Game>> GetGame(int id) {
            var game = await _context.Games
                .Include(g => g.Platform)
                .Include(g => g.Genres)
                .Include(g => g.GameImages)
                .Include(g => g.SystemRequirements)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) {
                return NotFound();
            }

            return game;
        }

        // Create a new game
        [HttpPost]
        public async Task<ActionResult<Game>> CreateGame(Game game) {
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
