using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Models.DTOs {
    public class FiltersGamesDTO {
        [Required]
        public int[]? GenresIds { get; set; } // category ids
        [Required]
        public double? RatingMin { get; set; }
        [Required]
        public double? RatingMax { get; set; }

    }
}
