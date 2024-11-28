using BackendGameVibes.Models.Steam;

namespace BackendGameVibes.IServices {
    public interface ISteamService {
        SteamApp[]? FindSteamApp(string name);
        Task<SteamApp[]?> GetAllGameIds();
        Task<GameData?> GetInfoGame(int id);
        Task InitSteamApi();
    }
}