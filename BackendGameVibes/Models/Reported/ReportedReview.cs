using BackendGameVibes.Models.User;

namespace BackendGameVibes.Models.Reported {
    public class ReportedReview {
        public int Id { get; set; }
        public string? Reason { get; set; }
        public int ReviewId { get; set; }
        public Review? Review { get; set; }
        public string? ReporterUserId { get; set; }
        public UserGameVibes? ReporterUser { get; set; }
    }
}
