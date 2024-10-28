namespace BackendGameVibes.Models.Games {
    public class Platform {
        public int Id {
            get; set;
        }
        public string? Name {
            get; set;
        }

        public ICollection<Game>? Games {
            get; set;
        }
    }
}
