using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.User;

namespace BackendGameVibes.Models.Reviews {
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
        public double AverageRating {
            get; set;
        }
        public string? Comment {
            get; set;
        }
        public DateTime? CreatedAt {
            get; set;
        }
        public DateTime? UpdatedAt {
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
