namespace BackendGameVibes.Models.Requests {
    public class ReportReviewDTO {
        public string? Reason { get; set; }
        public int ReviewId { get; set; }
        public string? ReporterUserId { get; set; }
    }
}
