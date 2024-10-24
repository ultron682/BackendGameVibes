using BackendGameVibes.Models.Forum;
using System.ComponentModel.DataAnnotations;

namespace BackendGameVibes.Models.Requests.Forum {
    public class ForumPostDTO {
        [Required]
        public string? Content {
            get; set;
        }
        [Required]
        public int? ThreadId {
            get; set;
        }
        [Required]
        public string? UserOwnerId {
            get; set;
        }
    }
}
