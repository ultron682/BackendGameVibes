using BackendGameVibes.IServices;
using BackendGameVibes.Models.Steam;
using System.Text.Json;

namespace BackendGameVibes.Services;


public class SteamService : ISteamService {
    public SteamApp[]? steamGames = null; // only steamID and name!!!
    private readonly HttpClient _httpClient;


    public SteamService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task InitSteamApi() {
        Console.WriteLine("Start init Steam games");
        steamGames = await GetAllGameIds();
        Console.WriteLine("Finish init Steam games");
    }

    public SteamApp[]? FindSteamApp(string name) {
        name = name.ToLower();
        if (steamGames != null) {
            return steamGames.Where(s => s.Name!.ToLower().Contains(name)).Select(s => s).ToArray();
        }
        else {
            Console.WriteLine("SteamGames empty!!!!");
            return null;
        }
    }

    public async Task<SteamApp[]?> GetAllGameIds() {
        try {
            string filePath = "steam_games.json";
            if (File.Exists(filePath)) {
                var jsonString = await File.ReadAllTextAsync(filePath);
                var options = new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                };
                var appListWrapper = JsonSerializer.Deserialize<AppListWrapper>(jsonString, options);
                return appListWrapper!.Applist.Apps;
            }
            else {
                var response = await _httpClient.GetAsync("https://api.steampowered.com/ISteamApps/GetAppList/v0002/?key=STEAMKEY&format=json");
                if (response.IsSuccessStatusCode) {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions {
                        PropertyNameCaseInsensitive = true,
                    };
                    var appListWrapper = JsonSerializer.Deserialize<AppListWrapper>(jsonString, options);
                    await File.WriteAllTextAsync(filePath, jsonString);
                    return appListWrapper!.Applist.Apps;
                }
            }
        }
        catch (Exception e) {
            Console.WriteLine("Error when downloading steam ids with titles");
            Console.WriteLine(e.Message);
        }
        return null;
    }

    public async Task<GameData?> GetInfoGame(int id) {
        try {
            var response = await _httpClient.GetAsync($"https://store.steampowered.com/api/appdetails?appids={id}&l=polish");

            if (response.IsSuccessStatusCode) {
                var jsonString = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<Dictionary<string, SteamAppData>>(jsonString, options);
                if (result != null) {
                    if (result[id.ToString()].Success) {
                        Console.WriteLine("Downloaded game: " + id);
                        return result[id.ToString()].Data;
                    }
                }
                else {
                    Console.WriteLine("Game not found: " + id);
                    return null;
                }
            }
        }
        catch {
            Console.WriteLine("Error when downloading steam id = " + id);
            return null;
        }

        return null;
    }
}
