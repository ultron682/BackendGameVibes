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
        public string type { get; set; }
        public string name { get; set; }
        public int steam_appid { get; set; }
        public bool is_free { get; set; }
        public string detailed_description { get; set; }
        public string about_the_game { get; set; }
        public string short_description { get; set; }
        public string supported_languages { get; set; }
        public string header_image { get; set; }
        public object website { get; set; }
        //public SystemRequirements? pc_requirements { get; set; }
        //public SystemRequirements? mac_requirements { get; set; }
        //public SystemRequirements? linux_requirements { get; set; }
        public string[] developers { get; set; }
        public string[] publishers { get; set; }
        public Platforms platforms { get; set; }
        public Category[] categories { get; set; }
        public Genre[] genres { get; set; }
        public Screenshot[] screenshots { get; set; }
        public Release_Date release_date { get; set; }



        public override string ToString() {
            return JsonSerializer.Serialize(this);
        }
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
        public string id { get; set; }
        public string description { get; set; }
    }

    public class Screenshot {
        public int Id { get; set; }
        public string path_thumbnail { get; set; }
        public string path_full { get; set; }
    }

    public class Release_Date {
        public bool ComingSoon { get; set; }
        public string Date { get; set; }
    }
}
