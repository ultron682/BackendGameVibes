using Microsoft.AspNetCore.Identity;
using System.Data;

namespace BackendGameVibes.Models
{
    public class UserGameVibes : IdentityUser
    {
        public byte[]? ProfilePicture
        {
            get; set;
        }
        public string? Description
        {
            get; set;
        }
        public int? ExperiencePoints
        {
            get; set;
        }
        //public string? RoleId
        //{
        //    get; set;
        //}
        public int? ForumRoleId
        {
            get; set;
        }

        //public IdentityRole? IdentityRole {
        //    get; set;
        //}
        public ForumRole? ForumRole {
            get; set;
        }
    }
}
