using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Steam;
using BackendGameVibes.Data;

namespace BackendGameVibes.IServices {
    public interface IGameService : IDisposable {
        Task<(Game?, bool)> CreateGame(int steamGameId);
        SteamApp[] FindSteamAppByName(string searchingName);
        SteamApp[] GetAllSteamIdsGames();
        Task<object?> GetGame(int id);
        Task<object[]> GetGames();
        Task<object[]> GetFilteredGames(FiltersGamesDTO filtersGamesDTO);
        Task<object[]> GetGenres();
        Task<object[]> GetLandingGames();
        Task<object[]> GetUpcomingGames();
        Task<IEnumerable<object>> GetGameReviews(int id);
        Task<(Game?, bool)[]> InitGamesBySteamIds(ApplicationDbContext applicationDbContext, HashSet<int> steamGamesToInitID);
    }
}