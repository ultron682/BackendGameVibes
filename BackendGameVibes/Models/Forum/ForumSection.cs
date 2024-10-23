namespace BackendGameVibes.Models.Forum {
    public class ForumSection {
        public int Id {
            get; set;
        }
        public string? Name {
            get; set;
        }

        public ICollection<ForumThread>? Threads {
            get; set;
        }
    }
}
