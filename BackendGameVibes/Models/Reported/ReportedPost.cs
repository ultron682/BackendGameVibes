using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.User;

namespace BackendGameVibes.Models.Reported {
    public class ReportedPost {
        public int Id { get; set; }
        public string? Reason { get; set; }
        public int ForumPostId { get; set; }
        public ForumPost? ForumPost { get; set; }
        public string? ReporterUserId { get; set; }
        public UserGameVibes? ReporterUser { get; set; }
    }
}
