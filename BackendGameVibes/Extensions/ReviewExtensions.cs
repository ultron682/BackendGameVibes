using BackendGameVibes.Models.Reviews;

namespace BackendGameVibes.Extensions {
    public static class ReviewExtensions {
        public static IQueryable<object> SelectReviewColumns(this IQueryable<Review> query) {
            return query.Select(r => new {
                r.Id,
                r.GeneralScore,
                r.GameplayScore,
                r.GraphicsScore,
                r.AudioScore,
                r.Comment,
                r.UserGameVibesId,
                r.CreatedAt,
                r.UpdatedAt,
                r.GameId,
                GameTitle = r.Game != null ? r.Game.Title : "NoData",
                GameCoverImageUrl = r.Game != null ? r.Game.CoverImage : "NoData",
                Username = r.UserGameVibes != null ? r.UserGameVibes.UserName : "NoUsername",
                UserForumRoleName = r.UserGameVibes != null ? r.UserGameVibes.ForumRole!.Name : "NoRank",
            });
        }
    }

}
