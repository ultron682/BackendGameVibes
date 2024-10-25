using BackendGameVibes.Models.Steam;
using System.Text.Json;

namespace BackendGameVibes.Services {
    public class SteamService {
        public SteamApp[]? steamGames = null; // only steamID and name!!!
        private readonly HttpClient _httpClient;


        public SteamService(HttpClient httpClient) {
            _httpClient = httpClient;
            InitSteamApi();
        }

        private async void InitSteamApi() {
            Console.WriteLine("Started init Steam games");
            steamGames = await GetAllGameIds();
            Console.WriteLine("Finished map Steam games");
        }

        public SteamApp[]? FindSteamApp(string name) {
            name = name.ToLower();
            if (steamGames != null) {
                return steamGames.Where(s => s.Name.ToLower().Contains(name)).Select(s => s).ToArray();
            }
            else {
                Console.WriteLine("SteamGames empty!!!!");
                return null;
            }
        }

        public async Task<SteamApp[]?> GetAllGameIds() {
            var response = await _httpClient.GetAsync("https://api.steampowered.com/ISteamApps/GetAppList/v0002/?key=STEAMKEY&format=json");

            if (response.IsSuccessStatusCode) {
                var jsonString = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                };


                var appListWrapper = JsonSerializer.Deserialize<AppListWrapper>(jsonString, options);
                return appListWrapper!.Applist.Apps;
            }

            return null;
        }

        public async Task<GameData?> GetInfoGame(int id) {
            var response = await _httpClient.GetAsync($"https://store.steampowered.com/api/appdetails?appids={id}");

            if (response.IsSuccessStatusCode) {
                var jsonString = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<Dictionary<string, SteamAppData>>(jsonString, options);

                if (result[id.ToString()].Success)
                    return result[id.ToString()].Data;
                else
                    return null;
            }

            return null;
        }
    }
}
