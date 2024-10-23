namespace BackendGameVibes.Models.Forum {
    public class ForumPost {
        public int Id {
            get; set;
        }
        public string? Content {
            get; set;
        }
        public DateTime CreatedDateTime {
            get; set;
        }
        public DateTime LastUpdatedDateTime {
            get; set;
        }
        public int? ThreadId {
            get; set;
        }
        public string? UserOwnerId {
            get; set;
        }
        public int Likes {
            get; set;
        }
        public int DisLikes {
            get; set;
        }

        public ForumThread? Thread {
            get; set;
        }
        public UserGameVibes? UserOwner {
            get; set;
        }

    }
}
