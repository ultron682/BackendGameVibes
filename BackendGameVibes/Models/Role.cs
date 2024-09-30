namespace BackendGameVibes.Models
{
    public class Role
    {
        public int Id
        {
            get; set;
        }
        public string? Name
        {
            get; set;
        }

        public ICollection<UserGameVibes>? Users
        {
            get; set;
        }
    }
}
