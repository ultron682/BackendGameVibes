using AutoMapper;
using BackendGameVibes.Models;
using BackendGameVibes.Models.Requests;

namespace BackendGameVibes.Helpers {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            CreateMap<ReviewDTO, Review>();
            CreateMap<Review, ReviewDTO>();
            CreateMap<UserGameVibes, RegisterDTO>();
            CreateMap<RegisterDTO, UserGameVibes>();
        }
    }
}
