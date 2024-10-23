using BackendGameVibes.Models.Forum;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace BackendGameVibes.Models {
    public class UserGameVibes : IdentityUser {
        public byte[]? ProfilePicture {
            get; set;
        }
        public string? Description {
            get; set;
        }
        public int? ExperiencePoints {
            get; set;
        }
        public int? ForumRoleId {
            get; set;
        }

        public List<Review> UserReviews {
            get; set;
        } = new List<Review>();
        public ForumRole? ForumRole {
            get; set;
        }
        public ICollection<Game>? FollowedGames {
            get; set;
        }
        public ICollection<ForumThread>? ForumThreads {
            get; set;
        }
    }
}
