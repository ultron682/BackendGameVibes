using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Steam;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Extensions;

namespace BackendGameVibes.Services;
public class GameService : IGameService {
    private readonly ApplicationDbContext _context;
    private readonly SteamService _steamService;


    public GameService(ApplicationDbContext context, SteamService steamService) {
        _context = context;
        _steamService = steamService;
    }

    public SteamApp[] FindSteamAppByName(string searchingName) {
        return _steamService.FindSteamApp(searchingName) ?? [];
    }

    public async Task<object?> GetFilteredGamesAsync(FiltersGamesDTO filtersGamesDTO, int pageNumber = 1, int resultSize = 10) {
        var query = _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .Where(g =>
                (filtersGamesDTO.GenresIds == null || !filtersGamesDTO.GenresIds.Any() || g.Genres!.Any(genre => filtersGamesDTO.GenresIds.Contains(genre.Id))) &&
                g.LastCalculatedRatingFromReviews >= filtersGamesDTO.RatingMin &&
                g.LastCalculatedRatingFromReviews <= filtersGamesDTO.RatingMax &&
                (string.IsNullOrEmpty(filtersGamesDTO.Title) || g.Title != null && g.Title.ToLower().Contains(filtersGamesDTO.Title.ToLower()))
            );
        

        int totalResults = await query.CountAsync();

        query = filtersGamesDTO.SortedBy switch {
            SortBy.Rating => filtersGamesDTO.IsSortedAscending
                ? query.OrderBy(g => g.LastCalculatedRatingFromReviews)
                : query.OrderByDescending(g => g.LastCalculatedRatingFromReviews),

            SortBy.Name => filtersGamesDTO.IsSortedAscending
                ? query.OrderBy(g => g.Title)
                : query.OrderByDescending(g => g.Title),

            SortBy.FollowedPlayers => filtersGamesDTO.IsSortedAscending
                ? query.OrderBy(g => g.PlayersFollowing!.Count)
                : query.OrderByDescending(g => g.PlayersFollowing!.Count),

            _ => filtersGamesDTO.IsSortedAscending
                ? query.OrderBy(g => g.ReleaseDate)
                : query.OrderByDescending(g => g.ReleaseDate) // SortBy.ReleaseDate and others
        };

        var data = await query
            .Skip((pageNumber - 1) * resultSize)
            .Take(resultSize)
            .Select(g => new {
                g.Id,
                g.Title,
                g.CoverImage,
                g.ReleaseDate,
                g.SteamId,
                Platforms = g.Platforms!.Select(p => new { p.Id, p.Name }).ToArray(),
                Genres = g.Genres!.Select(g => new { g.Id, g.Name }).ToArray(),
                Rating = g.LastCalculatedRatingFromReviews,
                RatingsCount = g.Reviews!.Count,
                PlayersFollowingCount = g.PlayersFollowing!.Count
            })
            .ToArrayAsync();


        return new {
            SortedBy = filtersGamesDTO.SortedBy.ToString()!.ToLower(),
            filtersGamesDTO.IsSortedAscending,
            TotalResults = totalResults,
            PageSize = resultSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalResults / (double)resultSize),
            Data = data
        };
    }

    public async Task<object?> GetGameDetailsAsync(int id) {
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
                g.ReleaseDate,
                g.SteamId,
                Platforms = g.Platforms!.Select(p => new { p.Id, p.Name }).ToArray(),
                Genres = g.Genres!.Select(g => new { g.Id, g.Name }).ToArray(),
                GameImages = g.GameImages!.Select(image => new { image.ImagePath }),
                Rating = g.LastCalculatedRatingFromReviews,
                RatingsCount = g.Reviews!.Count,
                PlayersFollowingCount = g.PlayersFollowing!.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(Game?, bool)[]> InitGamesBySteamIdsAsync(ApplicationDbContext applicationDbContext, HashSet<int> steamGamesToInitID) {
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

        await applicationDbContext.SaveChangesAsync();
        return resultsGames.ToArray();
    }

    private DateOnly ParseReleaseDate(string date) {
        try {
            DateTime parsedDate = DateTime.ParseExact(date, "d MMMM yyyy", new System.Globalization.CultureInfo("pl-PL"));
            return DateOnly.FromDateTime(parsedDate);
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

    public async Task<(Game?, bool)> AddGameAsync(int steamGameId) {
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
            DateTime parsedDate = DateTime.ParseExact(steamGameData.release_date.Date, "d MMMM yyyy", new System.Globalization.CultureInfo("pl-PL"));
            game.ReleaseDate = DateOnly.FromDateTime(parsedDate);
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

    public async Task<object[]> GetGenresAsync() {
        return await _context.Genres
            .Select(g => new {
                g.Id,
                g.Name
            })
            .ToArrayAsync();
    }

    public async Task<object[]> GetPlatformsAsync() {
        return await _context.Platforms
            .Select(g => new {
                g.Id,
                g.Name
            })
            .ToArrayAsync();
    }

    public async Task<object[]> GetLandingGamesAsync() {
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

    public async Task<object[]> GetUpcomingGamesAsync() {
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

    public async Task<object?> UpdateGameAsync(int gameId, GameUpdateDTO gameUpdateDTO) {
        if (gameUpdateDTO == null) {
            return null;
        }

        var game = await _context.Games
            .Include(g => g.Genres)
            .Include(g => g.Platforms)
            .Include(g => g.GameImages)
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

            if (gameUpdateDTO.GenresIds != null) {
                game.Genres = await _context.Genres.Where(g => gameUpdateDTO.GenresIds.Contains(g.Id)).ToListAsync();
            }

            if (gameUpdateDTO.PlatformsIds != null) {
                game.Platforms = await _context.Platforms.Where(p => gameUpdateDTO.PlatformsIds.Contains(p.Id)).ToListAsync();
            }

            if (gameUpdateDTO.GameImagesUrls != null) {
                game.GameImages = gameUpdateDTO.GameImagesUrls.Select(url => new GameImage { ImagePath = url }).ToList();
            }

            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }
        catch {
            return null;
        }

        return await GetGameDetailsAsync(game.Id);
    }

    public async Task<bool?> RemoveGameAsync(int gameId) {
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
