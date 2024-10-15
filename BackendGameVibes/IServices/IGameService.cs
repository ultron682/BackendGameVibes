using BackendGameVibes.Models;
using BackendGameVibes.SteamApiModels;

namespace BackendGameVibes.IServices {
    public interface IGameService {
        Task<Game?> CreateGame(int steamGameId);
        SteamApp[] FindSteamAppByName(string searchingName);
        Task<SteamApp[]> GetAllSteamIdsGames();
        Task<object?> GetGame(int id);
        Task<object[]> GetGames();
    }
}