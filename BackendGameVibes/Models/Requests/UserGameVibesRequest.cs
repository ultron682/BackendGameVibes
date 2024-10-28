using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Friends;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;

namespace BackendGameVibes.Models.Requests {
    public class UserGameVibesRequest : IdentityUser {
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
        public ForumRole? ForumRole {
            get; set;
        }
    }
}
