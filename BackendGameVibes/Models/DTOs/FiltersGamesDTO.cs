using BackendGameVibes.Models.Steam;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Models.DTOs {
    public enum SortBy {
        Rating = 1,
        ReleaseDate,
        Name,
        FollowedPlayers
    }

    public class FiltersGamesDTO {
        public int[]? GenresIds { get; set; } // category ids

        [Required]
        [DefaultValue("0.0")]
        [Range(0.0, 10.0)]
        public double? RatingMin { get; set; } = 0;

        [Required]
        [DefaultValue("10.0")]
        [Range(0.0, 10.0)]
        public double? RatingMax { get; set; } = 10;

        [DefaultValue("rating")]
        public SortBy? SortedBy { get; set; } = SortBy.Rating;

        [DefaultValue("true")]
        public bool IsSortedAscending { get; set; }

    }
}
