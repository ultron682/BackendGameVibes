namespace BackendGameVibes.Models
{
    public class ForumRole
    {
        public int Id
        {
            get; set;
        }
        public string? Name
        {
            get; set;
        }
        public int? Threshold
        {
            get; set;
        }

        public ICollection<UserGameVibes>? Users
        {
            get; set;
        }
    }
}
