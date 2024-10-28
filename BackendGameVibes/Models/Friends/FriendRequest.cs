using BackendGameVibes.Models.User;

namespace BackendGameVibes.Models.Friends
{
    public class FriendRequest {
        public int Id { get; set; }
        public string? SenderUserId { get; set; }
        public UserGameVibes? SenderUser { get; set; }

        public string? ReceiverUserId { get; set; }
        public UserGameVibes? ReceiverUser { get; set; }

        public DateTime SentAt { get; set; }
        public bool? IsAccepted { get; set; } = null;
    }
}
