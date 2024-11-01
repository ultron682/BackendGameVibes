using BackendGameVibes.Models.Forum;

namespace BackendGameVibes.Models.DTOs.Forum {
    public class ForumThreadDTO {
        public string? Title {
            get; set;
        }
        public string? UserOwnerId {
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
