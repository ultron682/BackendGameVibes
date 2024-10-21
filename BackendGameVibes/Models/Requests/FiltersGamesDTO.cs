namespace BackendGameVibes.Models.Requests {
    public class FiltersGamesDTO {
        public int[]? GenresIds { get; set; } // category ids
        public double? RatingMin { get; set; }
        public double? RatingMax { get; set; }

    }
}
