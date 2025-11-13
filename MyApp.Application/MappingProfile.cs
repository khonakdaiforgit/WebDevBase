using AutoMapper;
using MyApp.Application.Abstractions.Contacts.Dtos;
using MyApp.Application.Abstractions.Galleries.Dtos;
using MyApp.Application.Abstractions.Logging.Dtos;
using MyApp.Application.Abstractions.Menus.Dtos;
using MyApp.Application.Abstractions.News.Dtos;
using MyApp.Application.Abstractions.Newsletters.Dtos;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.Application.Abstractions.Subscribers.Dtos;
using MyApp.Application.Abstractions.Users.Dtos;
using MyApp.Domain.Entities;


namespace MyApp.Application
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ===== Restaurant =====
            CreateMap<Restaurant, RestaurantDto>();
            CreateMap<Restaurant, RestaurantListItemDto>();

            CreateMap<CreateRestaurantDto, Restaurant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl ?? string.Empty));

            CreateMap<UpdateRestaurantDto, Restaurant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // ===== MenuCategory & MenuItem =====
            CreateMap<MenuCategory, MenuCategoryDto>();
            CreateMap<MenuItem, MenuItemDto>();

            CreateMap<CreateCategoryDto, MenuCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            CreateMap<UpdateCategoryDto, MenuCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateMenuItemDto, MenuItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateMenuItemDto, MenuItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // ===== Gallery =====
            CreateMap<GalleryItem, GalleryItemDto>();

            CreateMap<UploadGalleryItemDto, GalleryItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UploadDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsVisible, opt => opt.MapFrom(src => true));

            CreateMap<UpdateGalleryItemDto, GalleryItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // ===== News =====
            CreateMap<Domain.Entities.News, NewsDto>();
            CreateMap<Domain.Entities.News, NewsListItemDto>();

            CreateMap<CreateNewsDto, Domain.Entities.News>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => false));

            CreateMap<UpdateNewsDto, Domain.Entities.News>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // ===== Newsletter =====
            CreateMap<Newsletter, NewsletterDto>();
            CreateMap<Newsletter, NewsletterListItemDto>();

            CreateMap<CreateNewsletterDto, Newsletter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => NewsletterStatus.Draft))
                .ForMember(dest => dest.SentAt, opt => opt.Ignore())
                .ForMember(dest => dest.SentByUserId, opt => opt.Ignore());

            CreateMap<UpdateNewsletterDto, Newsletter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // ===== EmailSubscriber =====
            CreateMap<EmailSubscriber, SubscriberDto>();

            CreateMap<SubscribeDto, EmailSubscriber>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SubscribedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false)) // نیاز به تأیید
                .ForMember(dest => dest.UnsubscribeToken, opt => opt.MapFrom(src => Guid.NewGuid().ToString()));

            // ===== ContactMessage =====
            CreateMap<ContactMessage, ContactMessageDto>();

            // ===== User =====
            CreateMap<User, UserDto>();

            // ===== LogEntry =====
            CreateMap<LogEntry, LogEntryDto>();
        }
    }
}