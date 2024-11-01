using AutoMapper;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Reported;
using BackendGameVibes.Models.Requests;
using BackendGameVibes.Models.Requests.Account;
using BackendGameVibes.Models.Requests.Forum;
using BackendGameVibes.Models.Requests.Reported;
using BackendGameVibes.Models.Reviews;
using BackendGameVibes.Models.User;

namespace BackendGameVibes.Helpers
{
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            CreateMap<ReviewDTO, Review>();
            CreateMap<Review, ReviewDTO>();
            CreateMap<UserGameVibes, RegisterDTO>();
            CreateMap<RegisterDTO, UserGameVibes>();
            CreateMap<ForumPost, ForumPostDTO>();
            CreateMap<ForumPostDTO, ForumPost>();
            CreateMap<ForumThreadDTO, ForumThread>();
            CreateMap<ForumThread, ForumThreadDTO>();
            CreateMap<NewForumThreadDTO, ForumThread>();
            CreateMap<ForumThread, NewForumThreadDTO>();
            CreateMap<ReportPostDTO, ReportedPost>()
                .ForMember(dest => dest.ForumPostId, opt => opt.MapFrom(src => src.ForumPostId));
            CreateMap<ReportedPost, ReportPostDTO>();
            CreateMap<ReportedReview, ReportReviewDTO>();
            CreateMap<ReportReviewDTO, ReportedReview>();
        }
    }
}
