using AutoMapper;
using UseCase.Contracts;
using UseCase.Web.Models;

namespace UseCase.Web.Helpers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Post, PostViewModel>().ReverseMap();
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<PrivacySettings, PrivacySettingsViewModel>().ReverseMap();
        }
    }
}