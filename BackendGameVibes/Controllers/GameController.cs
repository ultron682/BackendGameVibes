using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Models;
using BackendGameVibes.Data;
using BackendGameVibes.Services;
using BackendGameVibes.SteamApiModels;

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
                return Ok(_steamService.steamGames[29000..30000]); // example only small range
            else
                return NotFound("GetAllSteamIdsGames unsuccessful");
        }

        [HttpGet("search")]
        public async Task<ActionResult<SteamApp[]>> FindSteamAppByName(string searchingName) {
            var steamApp = _steamService.FindSteamApp(searchingName);
            if (steamApp != null)
                return Ok(steamApp);
            else
                return BadRequest("steamApp == null. maybe json with 500k lines is downloading...");
        }

        // Get all games
        [HttpGet]
        public async Task<ActionResult<object[]>> GetGames() {
            return await _context.Games
                .Include(g => g.Platforms)
                .Include(g => g.Genres)
                .Include(g => g.GameImages)
                .Include(g => g.SystemRequirements)
                .Select(g => new {
                    g.Id,
                    g.Title,
                    g.Description,
                    g.HeaderImage,
                    g.ReleaseDate,
                    g.SteamId,
                })
                .ToArrayAsync();
        }

        // Get a single game by ID
        [HttpGet("{id}")]
        public async Task<ActionResult> GetGame(int id) {
            var game = await _context.Games
                .Where(g => g.Id == id)
                .Include(g => g.Platforms)
                .Include(g => g.Genres)
                .Include(g => g.GameImages)
                .Include(g => g.SystemRequirements)
                .Select(g => new {
                    g.Id,
                    g.Title,
                    g.Description,
                    g.HeaderImage,
                    Platforms = g.Platforms.Select(p => new { p.Id, p.Name }),
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
                return Ok(game);
            }
        }

        /* swagger value for test
            {
              "title": "NIE WAZNE CO TU BEDZIE BO I TAK DANE Z STEAM po steamid TO NADPISZĄ",
              "description": "NIE WAZNE CO TU BEDZIE BO I TAK DANE Z STEAM TO NADPISZĄ",
              "steamid": 3111230
            }
         */

        // PODCZAS WPROWADZANIA GRY ALE PRZED JESZCZE WYWOŁANIEM TEJ METODU NALEZY PODAC NAZWE A WTEDY WSKAZAĆ ZNALEZIONĄ GRĘ.
        // W TEN SPOSÓB BĘDZIE UZYSKANY STEAMID I DZIĘKI TEMU POBIERZE SIE OPIS, RELEASEDATE I INNE DANE ZE STEAMA
        [HttpPost]
        public async Task<ActionResult<Game>> CreateGame(int steamGameId = 292030) {
            Game game = new Game() { SteamId = steamGameId };

            var steamGameData = await _steamService.GetInfoGame(game.SteamId);
            //Console.WriteLine(steamGameData);
            if(steamGameData == null) 
                return BadRequest("SteamGameData is null");

            game.Title = steamGameData.name;
            game.Description = steamGameData.detailed_description != null ? steamGameData.detailed_description : "Brak opisu";
            game.ReleaseDate = steamGameData.release_date.Date;
            game.HeaderImage = steamGameData.header_image;
            game.GameImages = steamGameData.screenshots.Select(s => new GameImage { ImagePath = s.path_full }).ToList();

            List<SteamApiModels.Genre> steamGenres = steamGameData.genres != null ? steamGameData.genres.ToList() : new List<SteamApiModels.Genre>();
            List<int> dbGenreIds = _context.Genres.Select(g => g.Id).ToList();


            var existingGenresInDB = await _context.Genres.Where(g => steamGenres.Select(s => int.Parse(s.id)).Contains(g.Id)).ToListAsync();

            foreach (var ele in existingGenresInDB)
                game.Genres.Add(ele);

            foreach (var steamGenre in steamGenres) {
                if (dbGenreIds.Contains(int.Parse(steamGenre.id)) == false) {
                    var newGenre = new Models.Genre { Id = int.Parse(steamGenre.id), Name = steamGenre.description };
                    _context.Genres.Add(newGenre);
                    if (game.Genres.Contains(newGenre) == false)
                        game.Genres.Add(newGenre);
                }
            }

            await _context.SaveChangesAsync();

            List<int> platformsIds = new List<int>();
            if (steamGameData.platforms.Windows)
                platformsIds.Add(1);
            else if (steamGameData.platforms.Windows)
                platformsIds.Add(2);
            else if (steamGameData.platforms.Mac)
                platformsIds.Add(3);

            game.Platforms = await _context.Platforms.Where(p => platformsIds.Contains(p.Id)).ToListAsync();

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

        [HttpGet("genre")]
        public async Task<IActionResult> GetAllGenres() {
            return Ok(await _context.Genres.Select(g => new { g.Id, g.Name }).ToArrayAsync());
        }
    }
}
