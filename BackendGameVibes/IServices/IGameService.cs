using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Steam;
using BackendGameVibes.Data;

namespace BackendGameVibes.IServices;
public interface IGameService : IDisposable {
    Task<object?> GetGames(int pageNumber = 1, int resultSize = 10);
    Task<object?> GetFilteredGames(FiltersGamesDTO filtersGamesDTO, int pageNumber = 1, int resultSize = 10);
    Task<(Game?, bool)> CreateGame(int steamGameId);
    SteamApp[] FindSteamAppByName(string searchingName);
    SteamApp[] GetAllSteamIdsGames();
    Task<object?> GetGame(int id);
    Task<object[]> GetGenres();
    Task<object[]> GetLandingGames();
    Task<object[]> GetUpcomingGames();
    Task<object?> GetGameReviews(int id, int pageNumber = 1, int resultSize = 10);
    Task<(Game?, bool)[]> InitGamesBySteamIds(ApplicationDbContext applicationDbContext, HashSet<int> steamGamesToInitID);
}