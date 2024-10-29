namespace BackendGameVibes.Models.Requests {
    public class ReportPostDTO {
        public string? Reason { get; set; }
        public int PostId { get; set; }
        public string? ReporterUserId { get; set; }
    }
}
