using AutoMapper;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Requests;
using BackendGameVibes.Models.Requests.Account;
using BackendGameVibes.Models.Requests.Forum;
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
            //                           .ForMember(dest => dest.CurrentCity,
            //                              opt => opt.MapFrom(src => src.City));
        }
    }
}
