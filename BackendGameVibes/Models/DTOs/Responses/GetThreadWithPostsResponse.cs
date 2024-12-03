namespace BackendGameVibes.Models.DTOs.Responses;

public class GetThreadWithPostsResponse {
    public int? NewForumThreadId { get; set; }
    public object? Thread { get; set; }
    public object? PostsOfThread { get; set; }
}
