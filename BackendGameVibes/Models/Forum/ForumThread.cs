namespace BackendGameVibes.Models.Forum {
    public class ForumThread {
        public int Id {
            get; set;
        }
        public string? Title {
            get; set;
        }
        public DateTime CreatedDateTime {
            get; set;
        }
        public DateTime LastUpdatedDateTime {
            get; set;
        }
        public string? UserOwnerId {
            get; set;
        }
        public int? SectionId {
            get; set;
        }

        public ICollection<ForumPost>? Posts {
            get; set;
        } = [];
        public UserGameVibes? UserOwner {
            get; set;
        }
        public ForumSection? Section {
            get; set;
        }
    }
}
