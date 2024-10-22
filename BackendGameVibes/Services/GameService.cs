﻿using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using BackendGameVibes.Models.Requests;
using BackendGameVibes.Models.Steam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
                    g.HeaderImage,
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
                    g.HeaderImage,
                    Platforms = g.Platforms!.Select(p => new { p.Id, p.Name }),
                    g.ReleaseDate,
                    g.SteamId,
                    Genres = g.Genres!.Select(g => new { g.Id, g.Name }).ToArray(),
                    GameImages = g.GameImages!.Select(image => new { image.ImagePath }),
                    Rating = g.Reviews!.Where(c => c.GameId == g.Id).Select(c => c.GameplayScore).Average().ToString("0.0")
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Game?> CreateGame(int steamGameId) {
            Game? foundGame = await _context.Games.Where(g => g.SteamId == steamGameId).FirstOrDefaultAsync();
            if (foundGame != null)
                return null;

            Game game = new() { SteamId = steamGameId };

            var steamGameData = await _steamService.GetInfoGame(game.SteamId);
            //Console.WriteLine(steamGameData);
            if (steamGameData == null)
                return null;

            game.Title = steamGameData.name;
            game.Description = steamGameData.detailed_description != null ? steamGameData.detailed_description : "Brak opisu";
            game.ReleaseDate = DateOnly.ParseExact(steamGameData.release_date.Date, "dd MMM, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            game.HeaderImage = steamGameData.header_image;
            game.GameImages = steamGameData.screenshots.Select(s => new GameImage { ImagePath = s.path_full }).ToList();

            List<Models.Steam.Genre> steamGenres = steamGameData.genres != null ? steamGameData.genres.ToList() : [];
            List<int> dbGenreIds = _context.Genres.Select(g => g.Id).ToList();


            var existingGenresInDB = await _context.Genres.Where(g => steamGenres.Select(s => int.Parse(s.id)).Contains(g.Id)).ToListAsync();

            foreach (var ele in existingGenresInDB)
                game.Genres!.Add(ele);

            foreach (var steamGenre in steamGenres) {
                if (dbGenreIds.Contains(int.Parse(steamGenre.id)) == false) {
                    var newGenre = new Models.Genre { Id = int.Parse(steamGenre.id), Name = steamGenre.description };
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

            return game;
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
                    g.HeaderImage,
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
            return await _context.Games
                            .Include(g => g.Reviews)
                            .Select(g => new {
                                g.Id,
                                g.Title,
                                g.HeaderImage,
                                Rating = g.Reviews!.Where(c => c.GameId == g.Id).Select(c => c.GameplayScore).Average().ToString("0.0")
                            })
                            .Take(5)
                            .ToArrayAsync();

        }
        public void Dispose() {
            _context.Dispose();
        }

        public async Task<object[]> GetUpcomingGames() {
            return await _context.Games
                 .Include(g => g.Reviews)
                 .Where(g => g.ReleaseDate > DateOnly.FromDateTime(DateTime.Now))
                 .Select(g => new {
                     g.Id,
                     g.Title,
                     g.HeaderImage,
                     Rating = g.Reviews!.Where(c => c.GameId == g.Id).Select(c => c.GameplayScore).Average().ToString("0.0")
                 })
                 .Take(5)
                 .ToArrayAsync();
        }
    }
}
