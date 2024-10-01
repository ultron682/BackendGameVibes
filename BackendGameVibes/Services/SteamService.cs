using Azure;
using Microsoft.Identity.Client;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace BackendGameVibes.Services {
    class SteamApp { // dla pobierania wszystkich mozliwych gier ze steama
        int appid;
        string? name;
    }

    public class SteamAppData {
        public bool Success { get; set; }
        public GameData Data { get; set; }

        public override string ToString() {
            return JsonSerializer.Serialize(this);
        }
    }

    public class GameData {
        public string Type { get; set; }
        public string Name { get; set; }
        public int SteamAppid { get; set; }
        public int RequiredAge { get; set; }
        public bool IsFree { get; set; }
        public string ControllerSupport { get; set; }
        public string DetailedDescription { get; set; }
        public string AboutTheGame { get; set; }
        public string ShortDescription { get; set; }
        public string SupportedLanguages { get; set; }
        public string HeaderImage { get; set; }
        public string CapsuleImage { get; set; }
        public string CapsuleImagev5 { get; set; }
        public object Website { get; set; }
        public SystemRequirements PcRequirements { get; set; }
        public SystemRequirements MacRequirements { get; set; }
        public object[] LinuxRequirements { get; set; }
        public string[] Developers { get; set; }
        public string[] Publishers { get; set; }
        public PriceOverview PriceOverview { get; set; }
        public int[] Packages { get; set; }
        public PackageGroup[] PackageGroups { get; set; }
        public Platforms Platforms { get; set; }
        public Category[] Categories { get; set; }
        public Genre[] Genres { get; set; }
        public Screenshot[] Screenshots { get; set; }
        public ReleaseDate ReleaseDate { get; set; }
        public SupportInfo SupportInfo { get; set; }
        public string Background { get; set; }
        public string BackgroundRaw { get; set; }
        public ContentDescriptors ContentDescriptors { get; set; }
        public Ratings Ratings { get; set; }
    }

    public class SystemRequirements {
        public string Minimum { get; set; }
        public string Recommended { get; set; }
    }

    public class PriceOverview {
        public string Currency { get; set; }
        public int Initial { get; set; }
        public int Final { get; set; }
        public int DiscountPercent { get; set; }
        public string InitialFormatted { get; set; }
        public string FinalFormatted { get; set; }
    }

    public class PackageGroup {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SelectionText { get; set; }
        public string SaveText { get; set; }
        public int DisplayType { get; set; }
        public string IsRecurringSubscription { get; set; }
        public Sub[] Subs { get; set; }
    }

    public class Sub {
        public int Packageid { get; set; }
        public string PercentSavingsText { get; set; }
        public int PercentSavings { get; set; }
        public string OptionText { get; set; }
        public string OptionDescription { get; set; }
        public string CanGetFreeLicense { get; set; }
        public bool IsFreeLicense { get; set; }
        public int PriceInCentsWithDiscount { get; set; }
    }

    public class Platforms {
        public bool Windows { get; set; }
        public bool Mac { get; set; }
        public bool Linux { get; set; }
    }

    public class Category {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class Genre {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public class Screenshot {
        public int Id { get; set; }
        public string path_thumbnail { get; set; }
        public string path_full { get; set; }
    }

    public class ReleaseDate {
        public bool ComingSoon { get; set; }
        public string Date { get; set; }
    }

    public class SupportInfo {
        public string Url { get; set; }
        public string Email { get; set; }
    }

    public class ContentDescriptors {
        public object[] Ids { get; set; }
        public object Notes { get; set; }
    }

    public class Ratings {
        public RatingDetails Dejus { get; set; }
        public RatingDetails SteamGermany { get; set; }
    }

    public class RatingDetails {
        public string RatingGenerated { get; set; }
        public string Rating { get; set; }
        public string RequiredAge { get; set; }
        public string Banned { get; set; }
        public string UseAgeGate { get; set; }
        public string Descriptors { get; set; }
    }


    public class SteamService(HttpClient httpClient) {
        public async void GetAllGameIds() {
            var data = httpClient.GetAsync("https://api.steampowered.com/ISteamApps/GetAppList/v0002/?key=STEAMKEY&format=json");

        }

        public async Task<SteamAppData> GetInfoGame(int id) {
            var response = await httpClient.GetAsync($"https://store.steampowered.com/api/appdetails?appids={id}");


            // Sprawdzenie, czy odpowiedź była sukcesem
            if (response.IsSuccessStatusCode) {
                // Odczyt odpowiedzi jako string
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserializacja JSON do obiektu SteamAppData
                var options = new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true // Ignorowanie wielkości liter w nazwach właściwości
                };

                // Deserializacja i wyciągnięcie obiektu na podstawie ID
                var result = JsonSerializer.Deserialize<Dictionary<string, SteamAppData>>(jsonString, options);

                // Zwrócenie właściwego obiektu SteamAppData
                return result[id.ToString()];
            }

            return null;
        } 
    }
}
