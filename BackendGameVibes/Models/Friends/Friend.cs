namespace BackendGameVibes.Models.Friends {
    public class Friend {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public UserGameVibes? User { get; set; }

        public string? FriendId { get; set; }
        public UserGameVibes? FriendUser { get; set; }

        public DateTime FriendsSince { get; set; }
    }
}
