namespace BackendGameVibes.Models.User {
    public class ProfilePicture {
        public int Id { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageFormat { get; set; } // "jpg" || "png"
        public string? UserId { get; set; }
        public UserGameVibes? User { get; set; }
        public DateTime? UploadedDate { get; set; } = DateTime.UtcNow;
    }
}
