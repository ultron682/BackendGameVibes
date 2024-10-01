using System.Text.Json;

namespace BackendGameVibes.SteamApiModels {
    //for many games
    public class SteamApp { // dla pobierania wszystkich mozliwych gier ze steama
        public int Appid { get; set; }
        public string? Name { get; set; }

        public override string ToString() {
            return $"{Appid}: {Name}";
        }
    }

    // Klasa do deserializacji JSON
    public class AppListWrapper {
        public AppList Applist { get; set; }
    }

    public class AppList {
        public SteamApp[] Apps { get; set; }
    }


    // for one game
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
        public Platforms Platforms { get; set; }
        public Category[] Categories { get; set; }
        public Genre[] Genres { get; set; }
        public Screenshot[] Screenshots { get; set; }
        public ReleaseDate ReleaseDate { get; set; }
    }

    public class SystemRequirements {
        public string Minimum { get; set; }
        public string Recommended { get; set; }
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
}
