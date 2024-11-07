using BackendGameVibes.Models.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendGameVibes.Models.Forum {
    public class ForumPost {
        public int Id {
            get; set;
        }
        public string? Content {
            get; set;
        }
        public DateTime CreatedDateTime {
            get; set;
        }
        public DateTime LastUpdatedDateTime {
            get; set;
        }
        public int? ThreadId {
            get; set;
        }
        public string? UserOwnerId {
            get; set;
        }
        public int LikesCount {
            get; set;
        }
        public int DisLikesCount {
            get; set;
        }

        public ForumThread? Thread {
            get; set;
        }
        public UserGameVibes? UserOwner {
            get; set;
        }
        public ICollection<Reported.ReportedPost>? ReportsToThisPosts {
            get; set;
        } = [];
        public ICollection<ForumPostInteraction>? PostInteractions {
            get; set;
        } = [];
    }
}
