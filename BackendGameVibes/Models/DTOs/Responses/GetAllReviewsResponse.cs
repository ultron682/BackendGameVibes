namespace BackendGameVibes.Models.DTOs.Responses;

public class GetAllReviewsResponse {
    public int TotalResults { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public object[]? Data { get; set; }
}
