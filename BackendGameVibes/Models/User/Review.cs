using BackendGameVibes.Models.Games;

namespace BackendGameVibes.Models.User {
    public class Review {
        public int Id {
            get; set;
        }
        public string? UserGameVibesId {
            get; set;
        }
        public int? GameId {
            get; set;
        }
        public double GeneralScore {
            get; set;
        }
        public double GraphicsScore {
            get; set;
        }
        public double AudioScore {
            get; set;
        }
        public double GameplayScore {
            get; set;
        }
        public string? Comment {
            get; set;
        }
        public DateTime? CreatedAt {
            get; set;
        }

        public UserGameVibes? UserGameVibes {
            get; set;
        }
        public Game? Game {
            get; set;
        }
        public ICollection<Reported.ReportedReview>? ReportedReviews {
            get; set;
        } = [];
    }
}
