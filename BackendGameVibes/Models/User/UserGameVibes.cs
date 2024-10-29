using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Friends;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.Reported;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace BackendGameVibes.Models.User {
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
        public ICollection<Game>? UserFollowedGames {
            get; set;
        } = [];
        public ICollection<ForumThread>? UserForumThreads {
            get; set;
        } = [];
        public ICollection<ForumPost>? UserForumPosts {
            get; set;
        } = [];
        public ICollection<FriendRequest> FriendRequestsSent {
            get; set;
        } = [];
        public ICollection<FriendRequest> FriendRequestsReceived {
            get; set;
        } = [];
        public ICollection<Friend> Friends {
            get; set;
        } = [];
        public ICollection<ReportedReview> ReportedReviews {
            get; set;
        } = [];
        public ICollection<ReportedPost> ReportedPosts {
            get; set;
        } = [];
    }
}
