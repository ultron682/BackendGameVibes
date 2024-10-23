using BackendGameVibes.Models.Forum;

namespace BackendGameVibes.Models.Requests {
    public class ForumThreadDTO {
        public string? Title {
            get; set;
        }
        public string? UserIdOwner {
            get; set;
        }
        public int? SectionId {
            get; set;
        }
        public ForumPostDTO? FirstPost {
            get; set;
        }
    }
}
