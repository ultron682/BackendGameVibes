using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Steam;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BackendGameVibes.Models.Reviews;

namespace BackendGameVibes.Services;
public class GameService : IGameService {
    private readonly ApplicationDbContext _context;
    private readonly SteamService _steamService;


    public GameService(ApplicationDbContext context, SteamService steamService) {
        _context = context;
        _steamService = steamService;
    }

    public SteamApp[] GetAllSteamIdsGames() {
        return _steamService.steamGames![29000..30000];
    }

    public SteamApp[] FindSteamAppByName(string searchingName) {
        return _steamService.FindSteamApp(searchingName) ?? [];
    }

    public async Task<object?> GetGames(int pageNumber = 1, int resultSize = 10) {
        var query = await _context.Games
            .OrderByDescending(t => t.ReleaseDate)
            .Skip((pageNumber - 1) * resultSize)
            .Take(resultSize)
            .Include(g => g.Genres)
            .Select(g => new {
                g.Id,
                g.Title,
                g.Description,
                g.CoverImage,
                g.ReleaseDate,
                g.SteamId,
                Rating = g.LastCalculatedRatingFromReviews,
                RatingsCount = g.Reviews!.Count
            })
            .ToArrayAsync();

        int totalResults = await _context.Games.CountAsync();

        return new {
            TotalResults = totalResults,
            PageSize = resultSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalResults / (double)resultSize),
            Data = query
        };
    }

    public async Task<object?> GetFilteredGames(FiltersGamesDTO filtersGamesDTO, int pageNumber = 1, int resultSize = 10) {
        var query = await _context.Games
            .Include(g => g.Genres)
            .Where(g =>
                (filtersGamesDTO.GenresIds == null || !filtersGamesDTO.GenresIds.Any() || g.Genres!.Any(genre => filtersGamesDTO.GenresIds.Contains(genre.Id))) &&
                g.LastCalculatedRatingFromReviews >= filtersGamesDTO.RatingMin &&
                g.LastCalculatedRatingFromReviews <= filtersGamesDTO.RatingMax
            )
            .OrderByDescending(t => t.ReleaseDate)
            .Skip((pageNumber - 1) * resultSize)
            .Take(resultSize)
            .Select(g => new {
                g.Id,
                g.Title,
                g.CoverImage,
                g.ReleaseDate,
                g.SteamId,
                Rating = g.LastCalculatedRatingFromReviews,
                RatingsCount = g.Reviews!.Count
            })
            .ToArrayAsync();

        int totalResults = await _context.Games
            .Where(g => filtersGamesDTO.GenresIds!.Contains(g.Genres!.Select(g => g.Id).FirstOrDefault())
            && g.LastCalculatedRatingFromReviews >= filtersGamesDTO.RatingMin
            && g.LastCalculatedRatingFromReviews <= filtersGamesDTO.RatingMax)
            .CountAsync();

        return new {
            TotalResults = totalResults,
            PageSize = resultSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalResults / (double)resultSize),
            Data = query
        };
    }


    public async Task<object?> GetGame(int id) {
        return await _context.Games
            .Include(g => g.Platforms)
            .Include(g => g.Genres)
            .Include(g => g.GameImages)
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
                Rating = g.LastCalculatedRatingFromReviews
            })
            .FirstOrDefaultAsync();
    }

    public async Task<object?> GetGameReviews(int gameId, int pageNumber = 1, int resultSize = 10) {
        var query = await _context.Games
            .Include(g => g.Reviews)
            .Where(g => g.Id == gameId)
            .Select(g => new {
                g.Id,
                g.Title,
                Reviews = g.Reviews!
                    .Where(r => r.GameId == g.Id)
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((pageNumber - 1) * resultSize)
                    .Take(resultSize)
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

        int totalResults = await _context.Reviews!.Where(r => r.GameId == gameId).CountAsync();

        return new {
            TotalResults = totalResults,
            PageSize = resultSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalResults / (double)resultSize),
            Data = query
        };
    }


    public async Task<(Game?, bool)[]> InitGamesBySteamIds(ApplicationDbContext applicationDbContext, HashSet<int> steamGamesToInitID) {
        var tasks = steamGamesToInitID
            .Select(_steamService.GetInfoGame)
            .ToArray();

        var combinedResults = await Task.WhenAll(tasks);

        var existingGames = applicationDbContext.Games
            .Where(g => steamGamesToInitID.Contains(g.SteamId))
            .ToDictionary(g => g.SteamId);

        var dbGenres = applicationDbContext.Genres.ToList();

        var resultsGames = new List<(Game?, bool)>();

        foreach (var (steamGameId, steamGameData) in steamGamesToInitID.Zip(combinedResults, (id, data) => (id, data))) {
            Game? foundGame = existingGames.TryGetValue(steamGameId, out var gameInDb) ? gameInDb : null;

            if (foundGame != null) {
                resultsGames.Add((foundGame, true));
                continue;
            }

            Game newGame = new Game {
                SteamId = steamGameId,
                Title = steamGameData?.name ?? "Brak tytułu",
                Description = steamGameData?.detailed_description ?? "Brak opisu",
                CoverImage = @$"https://steamcdn-a.akamaihd.net/steam/apps/{steamGameId}/library_600x900_2x.jpg",
                ReleaseDate = ParseReleaseDate(steamGameData?.release_date.Date),
                GameImages = steamGameData?.screenshots?.Select(s => new GameImage { ImagePath = s.path_full }).ToList() ?? new List<GameImage>(),
                Genres = [],
            };

            // Przypisanie gatunków
            var gameGenres = steamGameData?.genres?
                .Select(g => new Models.Games.Genre { Id = int.Parse(g.id), Name = g.description })
                .ToList() ?? new List<Models.Games.Genre>();

            foreach (var genre in gameGenres) {
                var existingGenre = dbGenres.FirstOrDefault(g => g.Id == genre.Id);
                if (existingGenre == null) {
                    applicationDbContext.Genres.Add(genre);
                    dbGenres.Add(genre);
                    newGame.Genres.Add(genre);
                }
                else {
                    newGame.Genres.Add(existingGenre);
                }
            }

            // Przypisanie platform - dodaj tylko istniejące platformy
            var platformIds = GetPlatformIds(steamGameData?.platforms);
            var existingPlatforms = applicationDbContext.Platforms.Where(p => platformIds.Contains(p.Id)).ToList();
            newGame.Platforms = existingPlatforms;

            applicationDbContext.Games.Add(newGame);
            resultsGames.Add((newGame, true));
            Console.WriteLine("Added game: " + newGame.Title);
        }

        // Zapisanie zmian w bazie, aby zapewnić, że `Genres` są dodane przed `Games`.
        await applicationDbContext.SaveChangesAsync();
        return resultsGames.ToArray();
    }

    private DateOnly ParseReleaseDate(string date) {
        try {
            return DateOnly.ParseExact(date, "d MMM, yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }
        catch {
            return DateOnly.FromDateTime(DateTime.Now);
        }
    }

    private List<int> GetPlatformIds(dynamic platforms) {
        var platformIds = new List<int>();
        try {
            if (platforms.Windows)
                platformIds.Add(1);
            if (platforms.Mac)
                platformIds.Add(2);
            if (platforms.Linux)
                platformIds.Add(3);
        }
        catch {

        }

        return platformIds;
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
                        .OrderBy(r => EF.Functions.Random())
                        .Take(5)
                        .Select(g => new {
                            g.Id,
                            g.Title,
                            g.CoverImage,
                            Rating = g.LastCalculatedRatingFromReviews
                        })
                        .ToArrayAsync();

        return games;
    }

    public async Task<object[]> GetUpcomingGames() {
        return await _context.Games
             .Where(g => g.ReleaseDate > DateOnly.FromDateTime(DateTime.Now))
             .OrderBy(r => EF.Functions.Random())
             .Take(5)
             .Select(g => new {
                 g.Id,
                 g.Title,
                 g.CoverImage,
                 g.ReleaseDate
             })
            .ToArrayAsync();
    }

    public async Task<object?> UpdateGame(int gameId, GameUpdateDTO gameUpdateDTO) {
        if (gameUpdateDTO == null) {
            return null;
        }

        var game = await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null) {
            return null;
        }

        try {
            game.Title = gameUpdateDTO.Title ?? game.Title;
            game.Description = gameUpdateDTO.Description ?? game.Description;
            game.ReleaseDate = gameUpdateDTO.ReleaseDate ?? game.ReleaseDate;
            game.SteamId = gameUpdateDTO.SteamId ?? game.SteamId;
            game.CoverImage = gameUpdateDTO.CoverImage ?? game.CoverImage;
            game.LastCalculatedRatingFromReviews = gameUpdateDTO.LastCalculatedRatingFromReviews ?? game.LastCalculatedRatingFromReviews;

            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }
        catch {
            return null;
        }

        return game;
    }

    public async Task<bool?> RemoveGame(int gameId) {
        var game = await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null) {
            return null;
        }

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
        return true;
    }

    public void Dispose() {
        _context.Dispose();
    }
}
