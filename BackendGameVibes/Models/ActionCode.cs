namespace BackendGameVibes.Models {
    public class ActionCode {
        public int Id { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public string? UserId { get; set; }
    }
}
