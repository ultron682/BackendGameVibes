using BackendGameVibes.Models.User;

namespace BackendGameVibes.Models.Forum {
    public class ForumPostInteraction {
        public int Id { get; set; }
        public int? PostId { get; set; }
        public string? UserId { get; set; }
        public bool? IsLike { get; set; } = null;// true = like, false = dislike, null = no interaction, removed

        public ForumPost? ForumPost { get; set; }
        public UserGameVibes? UserGameVibes { get; set; }
    }
}
