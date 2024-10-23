using BackendGameVibes.Models.Forum;

namespace BackendGameVibes.Models.Requests {
    public class ForumPostDTO {
        public string? Content {
            get; set;
        }
        public int? ThreadId {
            get; set;
        }
        public string? UserOwnerId {
            get; set;
        }
    }
}
