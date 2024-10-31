namespace BackendGameVibes.IServices
{
    public interface IForumExperienceService
    {
        Task<int?> AddPostPoints(string userId);
        Task<int?> AddReviewPoints(string userId);
        Task<int?> AddThreadPoints(string userId);
    }
}