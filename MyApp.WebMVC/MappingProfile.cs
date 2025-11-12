using AutoMapper;
using MyApp.Application.DTOs.User;
using MyApp.WebMVC.Models;

namespace MyApp.WebMVC
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, UserProfileViewModel>().ReverseMap();
            CreateMap<FileLinkDto, FileLinkViewModel>()
                       .ForMember(dest => dest.StatusDisplay, opt =>
                           opt.MapFrom(src => src.Status.ToString()))
                       .ReverseMap();
            CreateMap<UserProfileDto, UserDashboardDataViewModel>().ReverseMap();
            CreateMap<TransactionDto, TransactionViewModel>().ReverseMap();
            CreateMap<PagedResultDto<TransactionDto>, PagedViewModel<TransactionViewModel>>().ReverseMap();

            CreateMap<ContactMessageCreateViewModel, ContactMessageCreateDto>();
            CreateMap<ContactMessageDto, ContactMessageViewModel>();
            CreateMap<ContactMessageViewModel, ContactMessageCreateDto>();
            CreateMap<ContactMessageViewModel, ContactMessageUpdateDto>();
            CreateMap<UserAffiliateDataViewModel, UserAffiliateDataDto>().ReverseMap();

        }
    }
}
