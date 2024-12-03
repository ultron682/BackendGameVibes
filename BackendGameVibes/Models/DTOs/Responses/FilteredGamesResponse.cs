using BackendGameVibes.Models.Steam;

namespace BackendGameVibes.Models.DTOs.Responses {
    public class FilteredGamesResponse {
        public string? SortedBy { get; set; }
        public bool IsSortedAscending { get; set; }
        public int TotalResults { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public object[]? Data { get; set; }
    }
}
