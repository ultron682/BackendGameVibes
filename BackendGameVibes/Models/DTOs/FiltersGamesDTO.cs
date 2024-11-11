using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Models.DTOs {
    public class FiltersGamesDTO {
        public int[]? GenresIds { get; set; } // category ids
        [Required]
        [DefaultValue("0.0")]
        [Range(0.0, 10.0)]
        public double? RatingMin { get; set; }
        [Required]
        [DefaultValue("10.0")]
        [Range(0.0, 10.0)]
        public double? RatingMax { get; set; }

    }
}
