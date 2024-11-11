using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.Reviews;
using BackendGameVibes.Models.User;

namespace BackendGameVibes.Models.DTOs;

public class GameUpdateDTO {
    public string? Title {
        get; set;
    }
    public string? Description {
        get; set;
    }
    public DateOnly? ReleaseDate {
        get; set;
    }
    public int? SteamId {
        get; set;
    }
    public string? CoverImage {
        get; set;
    }
    public double? LastCalculatedRatingFromReviews {
        get; set;
    }
    public ICollection<int>? PlatformsIds {
        get; set;
    }
    public ICollection<int>? GenresIds {
        get; set;
    }
    public ICollection<string>? GameImagesUrls {
        get; set;
    }
}
