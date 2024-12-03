using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.DTOs;
using BackendGameVibes.Models.Steam;
using BackendGameVibes.Data;
using BackendGameVibes.Models.DTOs.Responses;

namespace BackendGameVibes.IServices;
public interface IGameService : IDisposable {
    Task<FilteredGamesResponse?> GetFilteredGamesAsync(FiltersGamesDTO filtersGamesDTO, int pageNumber = 1, int resultSize = 10);
    Task<(Game?, bool)> AddGameAsync(int steamGameId);
    SteamApp[] FindSteamAppByName(string searchingName);
    Task<object?> GetGameDetailsAsync(int id);
    Task<object[]> GetGenresAsync();
    Task<object[]> GetPlatformsAsync();
    Task<object[]> GetLandingGamesAsync();
    Task<object[]> GetUpcomingGamesAsync();
    Task<(Game?, bool)[]> InitGamesBySteamIdsAsync(ApplicationDbContext applicationDbContext, HashSet<int> steamGamesToInitID);
    Task<object?> UpdateGameAsync(int gameId, GameUpdateDTO gameUpdateDTO);
    Task<bool?> RemoveGameAsync(int gameId);
}