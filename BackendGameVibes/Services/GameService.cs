using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Steam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BackendGameVibes.Services {
    public class GameService : IGameService {
        private readonly ApplicationDbContext _context;
        private readonly SteamService _steamService;


        public GameService(ApplicationDbContext context, SteamService steamService) {
            _context = context;
            _steamService = steamService;
        }

        public SteamApp[] GetAllSteamIdsGames() {
            return _steamService.steamGames[29000..30000];
        }

        public SteamApp[] FindSteamAppByName(string searchingName) {
            return _steamService.FindSteamApp(searchingName) ?? [];
        }

        public async Task<object[]> GetGames() {
            return await _context.Games
                .Include(g => g.Platforms)
                .Include(g => g.Genres)
                .Include(g => g.GameImages)
                .Include(g => g.SystemRequirements)
                .Include(g => g.Reviews)
                .Select(g => new {
                    g.Id,
                    g.Title,
                    g.Description,
                    g.CoverImage,
                    g.ReleaseDate,
                    g.SteamId,
                    Rating = g.Reviews!.Where(c => c.GameId == g.Id).Select(c => c.GameplayScore).Average().ToString("0.0")
                })
                .ToArrayAsync();
        }

        public async Task<object?> GetGame(int id) {
            return await _context.Games
                .Include(g => g.Platforms)
                .Include(g => g.Genres)
                .Include(g => g.GameImages)
                .Include(g => g.SystemRequirements)
                .Where(g => g.Id == id)
                .Select(g => new {
                    g.Id,
                    g.Title,
                    g.Description,
                    g.CoverImage,
                    Platforms = g.Platforms!.Select(p => new { p.Id, p.Name }),
                    g.ReleaseDate,
                    g.SteamId,
                    Genres = g.Genres!.Select(g => new { g.Id, g.Name }).ToArray(),
                    GameImages = g.GameImages!.Select(image => new { image.ImagePath }),
                    Rating = g.Reviews!.Where(c => c.GameId == g.Id).Select(c => c.GameplayScore).Average().ToString("0.0")
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<object>> GetGameReviews(int id) {
            return await _context.Games
                .Include(g => g.Reviews)
                //.ThenInclude(g => g.UserGameVibes)
                .Where(g => g.Id == id)
                .Select(g => new {
                    g.Id,
                    g.Title,
                    Reviews = g.Reviews!.Where(r => r.GameId == g.Id)
                        .Select(r => new {
                            r.Id,
                            r.GeneralScore,
                            r.GameplayScore,
                            r.GraphicsScore,
                            r.AudioScore,
                            r.Comment,
                            Username = r.UserGameVibes != null ? r.UserGameVibes.UserName : "NoUsername",
                            r.CreatedAt,
                        })
                        .ToArray()
                })
                .ToArrayAsync();
        }


        public async Task<(Game?, bool)[]> InitGamesBySteamIds(ApplicationDbContext applicationDbContext, HashSet<int> steamGamesToInitID) {
            var tasks = new List<Task<GameData?>>();

            foreach (var gameId in steamGamesToInitID) {
                tasks.Add(_steamService.GetInfoGame(gameId));
            }

            var combinedResults = await Task.WhenAll(tasks);
            var results = combinedResults.Select(t => t);

            List<(Game?, bool)> resultsGames = [];


            for (int i = 0; i < steamGamesToInitID.Count; i++) {
                int steamGameId = steamGamesToInitID.ElementAt(i);
                var steamGameData = tasks[i].Result;

                Console.WriteLine("Start: " + steamGameId);

                Game? foundGame = applicationDbContext.Games.Where(g => g.SteamId == steamGameId).FirstOrDefault();
                if (foundGame != null)
                    resultsGames.Add((foundGame, true));

                Game game = new() { SteamId = steamGameId };

                if (steamGameData == null)
                    continue; //return (null, false);

                game.Title = steamGameData.name;
                game.Description = steamGameData.detailed_description != null ? steamGameData.detailed_description : "Brak opisu";

                try {
                    game.ReleaseDate = DateOnly.ParseExact(steamGameData.release_date.Date, "d MMM, yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch {
                    game.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
                }
                //game.CoverImage = steamGameData.header_image;
                game.CoverImage = @$"https://steamcdn-a.akamaihd.net/steam/apps/{game.SteamId}/library_600x900_2x.jpg";
                game.GameImages = steamGameData.screenshots.Select(s => new GameImage { ImagePath = s.path_full }).ToList();

                List<Models.Steam.Genre> steamGenres = steamGameData.genres != null ? steamGameData.genres.ToList() : [];
                List<int> dbGenreIds = applicationDbContext.Genres.Select(g => g.Id).ToList();


                var existingGenresInDB = applicationDbContext.Genres.Where(g => steamGenres.Select(s => int.Parse(s.id)).Contains(g.Id)).ToList();

                foreach (var ele in existingGenresInDB)
                    game.Genres!.Add(ele);

                foreach (var steamGenre in steamGenres) {
                    if (dbGenreIds.Contains(int.Parse(steamGenre.id)) == false) {
                        var newGenre = new Models.Games.Genre { Id = int.Parse(steamGenre.id), Name = steamGenre.description };
                        if (applicationDbContext.Genres.FirstOrDefault(g => g.Id == newGenre.Id) == null)
                            applicationDbContext.Genres.Add(newGenre);
                        if (game.Genres!.Contains(newGenre) == false)
                            game.Genres.Add(newGenre);
                    }
                }

                List<int> platformsIds = [];
                if (steamGameData.platforms.Windows)
                    platformsIds.Add(1);
                else if (steamGameData.platforms.Windows)
                    platformsIds.Add(2);
                else if (steamGameData.platforms.Mac)
                    platformsIds.Add(3);

                game.Platforms = applicationDbContext.Platforms.Where(p => platformsIds.Contains(p.Id)).ToList();

                applicationDbContext.Games.Add(game);


                Console.WriteLine("Added game: " + game.Title);

                //return (game, true);
                resultsGames.Add((game, true));
                applicationDbContext.SaveChanges();
            }


            await _context.SaveChangesAsync();

            return resultsGames.ToArray();
        }

        public async Task<(Game?, bool)> CreateGame(int steamGameId) {
            Game? foundGame = await _context.Games.Where(g => g.SteamId == steamGameId).FirstOrDefaultAsync();
            if (foundGame != null)
                return (foundGame, false);

            Game game = new() { SteamId = steamGameId };

            var steamGameData = await _steamService.GetInfoGame(game.SteamId);
            //Console.WriteLine(steamGameData);
            if (steamGameData == null)
                return (null, false);

            game.Title = steamGameData.name;
            game.Description = steamGameData.detailed_description != null ? steamGameData.detailed_description : "Brak opisu";
            try {
                game.ReleaseDate = DateOnly.ParseExact(steamGameData.release_date.Date, "d MMM, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch {
                game.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
            }
            //game.CoverImage = steamGameData.header_image;
            game.CoverImage = @$"https://steamcdn-a.akamaihd.net/steam/apps/{game.SteamId}/library_600x900_2x.jpg";
            game.GameImages = steamGameData.screenshots.Select(s => new GameImage { ImagePath = s.path_full }).ToList();

            List<Models.Steam.Genre> steamGenres = steamGameData.genres != null ? steamGameData.genres.ToList() : [];
            List<int> dbGenreIds = _context.Genres.Select(g => g.Id).ToList();


            var existingGenresInDB = await _context.Genres.Where(g => steamGenres.Select(s => int.Parse(s.id)).Contains(g.Id)).ToListAsync();

            foreach (var ele in existingGenresInDB)
                game.Genres!.Add(ele);

            foreach (var steamGenre in steamGenres) {
                if (dbGenreIds.Contains(int.Parse(steamGenre.id)) == false) {
                    var newGenre = new Models.Games.Genre { Id = int.Parse(steamGenre.id), Name = steamGenre.description };
                    _context.Genres.Add(newGenre);
                    if (game.Genres!.Contains(newGenre) == false)
                        game.Genres.Add(newGenre);
                }
            }

            await _context.SaveChangesAsync();

            List<int> platformsIds = [];
            if (steamGameData.platforms.Windows)
                platformsIds.Add(1);
            else if (steamGameData.platforms.Windows)
                platformsIds.Add(2);
            else if (steamGameData.platforms.Mac)
                platformsIds.Add(3);

            game.Platforms = await _context.Platforms.Where(p => platformsIds.Contains(p.Id)).ToListAsync();

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            Console.WriteLine("Added game: " + game.Title);

            return (game, true);
        }

        // todo rating
        public async Task<object[]> GetFilteredGames(FiltersGamesDTO filtersGamesDTO) {
            return await _context.Games
                .Include(g => g.Platforms)
                .Include(g => g.Genres)
                .Include(g => g.GameImages)
                .Include(g => g.SystemRequirements)
                .Where(g => filtersGamesDTO.GenresIds!.Contains(g.Genres!.Select(g => g.Id).FirstOrDefault()))
                .Select(g => new {
                    g.Id,
                    g.Title,
                    g.CoverImage,
                    g.ReleaseDate,
                    g.SteamId,
                    Rating = g.Reviews!.Where(c => c.GameId == g.Id).Select(c => c.GameplayScore).Average().ToString("0.0")
                })
                .ToArrayAsync();
        }

        public async Task<object[]> GetGenres() {
            return await _context.Genres
                .Select(g => new {
                    g.Id,
                    g.Name
                })
                .ToArrayAsync();
        }

        public async Task<object[]> GetLandingGames() {
            var games = await _context.Games
                            .Select(g => new {
                                g.Id,
                                g.Title,
                                g.CoverImage,
                            })
                            .OrderBy(r => EF.Functions.Random())
                            .Take(5)
                            .ToArrayAsync();

            var gamesWithRatings = new object[games.Length];
            for (int i = 0; i < games.Length; i++) {
                var game = games[i];
                var reviews = await _context.Reviews.Where(r => r.GameId == game.Id).ToArrayAsync();
                double rating = 0.0;
                if (reviews.Count() > 0)
                    rating = reviews.Select(r => r.GameplayScore).Average();

                gamesWithRatings[i] = new {
                    game.Id,
                    game.Title,
                    game.CoverImage,
                    Rating = rating.ToString("0.0")
                };
            }

            return gamesWithRatings;
        }

        public async Task<object[]> GetUpcomingGames() {
            return await _context.Games
                 .Where(g => g.ReleaseDate > DateOnly.FromDateTime(DateTime.Now))
                 .Select(g => new {
                     g.Id,
                     g.Title,
                     g.CoverImage,
                     g.ReleaseDate
                 })
                .OrderBy(r => EF.Functions.Random())
                .Take(5)
                .ToArrayAsync();
        }

        public void Dispose() {
            _context.Dispose();
        }
    }
}
