using AutoMapper;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.WebMVC.Views.Dashboard.ViewModels;
using MyApp.WebMVC.Views.Home.ViewModels;
using MyApp.WebMVC.Views.Restaurant.ViewModels;

namespace MyApp.WebMVC.Mapping
{
    public class MvcMappingProfile : Profile
    {
        public MvcMappingProfile()
        {
            CreateMap<PublicRestaurantDto, HomeIndexViewModel>();
            CreateMap<RestaurantDto, DashboardIndexViewModel>();

            CreateMap<RestaurantDto, RestaurantProfileViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl));

            CreateMap<RestaurantProfileViewModel, UpdateRestaurantDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.WorkingHours, opt => opt.Ignore());
        }
    }
}
