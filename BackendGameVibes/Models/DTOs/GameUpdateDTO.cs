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
}
