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
        public DateOnly? ReleaseDate {
            get; set;
        }
        public int SteamId {
            get; set;
        }
        public string? HeaderImage {
            get; set;
        }

        public ICollection<Platform>? Platforms {
            get; set;
        } = new HashSet<Platform>();
        public ICollection<Genre>? Genres {
            get; set;
        } = new HashSet<Genre>();
        public ICollection<GameImage>? GameImages {
            get; set;
        }
        public ICollection<SystemRequirement>? SystemRequirements {
            get; set;
        }
        public ICollection<Review>? Reviews {
            get; set;
        }
        public ICollection<UserGameVibes>? PlayersFollowing {
            get; set;
        }
    }
}
