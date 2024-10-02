namespace BackendGameVibes.Models {
    public class Game {
        public int Id {
            get; set;
        }
        public string? Title {
            get; set;
        }
        public string? Description {
            get; set;
        }
        public string? ReleaseDate {
            get; set;
        }
        public int? PlatformId {
            get; set;
        }
        public int SteamId {
            get; set;
        }

        public Platform? Platform {
            get; set;
        }
        public ICollection<Genre>? Genres {
            get; set;
        }
        public ICollection<GameImage>? GameImages {
            get; set;
        }
        public ICollection<SystemRequirement>? SystemRequirements {
            get; set;
        }
    }
}
