using BackendGameVibes.Models.Forum;
using Microsoft.AspNetCore.Identity;

namespace BackendGameVibes.Models.Requests.Account {
    public class UserGameVibesDTO : IdentityUser {
        private byte[]? ProfilePicture {
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
        public string? RoleName {
            get; set;
        }
        public string? Password {
            get; set;
        }
        public new bool? LockoutEnabled {
            get; set;
        }
        public new bool? PhoneNumberConfirmed {
            get; set;
        }
        public new int? AccessFailedCount {
            get; set;
        }

        private new bool? TwoFactorEnabled {
            get; set;
        }



        private new string? NormalizedEmail { get; set; }

        private new string? PasswordHash { get; set; }

        private new string? SecurityStamp { get; set; }

        private new string? ConcurrencyStamp { get; set; }

        private new DateTimeOffset? LockoutEnd { get; set; }
        private new string? NormalizedUserName {
            get; set;
        }
    }
}