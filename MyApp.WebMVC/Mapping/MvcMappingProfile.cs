using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.WebMVC.Views.Dashboard.ViewModels;
using MyApp.WebMVC.Views.Home.ViewModels;
using AutoMapper;

namespace MyApp.WebMVC.Mapping
{
    public class MvcMappingProfile : Profile
    {
        public MvcMappingProfile()
        {
            CreateMap<PublicRestaurantDto, HomeIndexViewModel>();
            CreateMap<RestaurantDto, DashboardIndexViewModel>();
        }
    }
}
