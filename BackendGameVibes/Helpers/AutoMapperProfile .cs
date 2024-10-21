using AutoMapper;
using BackendGameVibes.Models;
using BackendGameVibes.Models.Requests;

namespace BackendGameVibes.Helpers {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            CreateMap<ReviewRequest, Review>();
            CreateMap<Review, ReviewRequest>();
            CreateMap<UserGameVibes, RegisterRequest>();
            CreateMap<RegisterRequest, UserGameVibes>();
        }
    }
}
