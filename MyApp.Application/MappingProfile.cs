using AutoMapper;
using MyApp.Application.Abstractions.Contacts.Dtos;
using MyApp.Application.Abstractions.Logging.Dtos;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.Application.Abstractions.Restaurants.Mapping;
using MyApp.Application.Abstractions.Users.Dtos;
using MyApp.Domain.Entities;
using MyApp.Domain.ValueObjects;


namespace MyApp.Application
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            // ===== Restaurant =====
            // Entity → DTO (برای نمایش)
            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom<WorkingHoursResolver<RestaurantDto>>());

            CreateMap<IEnumerable<Restaurant>, IEnumerable<RestaurantDto>>();

            CreateMap<Restaurant, RestaurantListItemDto>();

            CreateMap<Restaurant, PublicRestaurantDto>()
                .ForMember(dest => dest.IsOpenNow, opt => opt.MapFrom(src => src.WorkingHours.IsOpenNow()))
                .ForMember(dest => dest.TodayHoursDisplay, opt => opt.MapFrom(src => src.WorkingHours.GetTodayHours()))
                .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom<WorkingHoursResolver<PublicRestaurantDto>>());



            // Create DTO → Entity
            CreateMap<CreateRestaurantDto, Restaurant>()
                .ForMember(dest => dest.Mian, opt => opt.Ignore())  // ← این خط
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OwnerUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                    Location.Create(src.Latitude, src.Longitude)))
                .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src =>
                    WorkingHours.Create(src.WorkingHours)));

            // Update DTO → Entity (فقط فیلدهای غیر null آپدیت بشن)
            CreateMap<UpdateRestaurantDto, Restaurant>()
              .ForMember(dest => dest.Mian, opt => opt.Ignore())  // ← این خط
              .ForMember(dest => dest.OwnerUserId, opt => opt.Ignore())
              .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                  src.Latitude.HasValue && src.Longitude.HasValue
                      ? Location.Create(src.Latitude.Value, src.Longitude.Value)
                      : null))
               .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Update Logo
            CreateMap<UpdateLogoDto, Restaurant>()
                .ForMember(dest => dest.Mian, opt => opt.Ignore())  // ← این خط
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForAllMembers(opt => opt.Ignore()); // فقط LogoUrl آپدیت بشه

            // Update Working Hours
            CreateMap<UpdateWorkingHoursDto, Restaurant>()
                .ForMember(dest => dest.Mian, opt => opt.Ignore())  // ← این خط
                .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src =>
                    WorkingHours.Create(src.WorkingHours)))
                .ForAllMembers(opt => opt.Ignore());

            // ===== MenuCategory & MenuItem =====
            //CreateMap<MenuCategory, MenuCategoryDto>();
            //CreateMap<MenuItem, MenuItemDto>();

            //CreateMap<CreateCategoryDto, MenuCategory>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.Items, opt => opt.Ignore());

            //CreateMap<UpdateCategoryDto, MenuCategory>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            //CreateMap<CreateMenuItemDto, MenuItem>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore());

            //CreateMap<UpdateMenuItemDto, MenuItem>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            //// ===== Gallery =====
            //CreateMap<GalleryItem, GalleryItemDto>();

            //CreateMap<UploadGalleryItemDto, GalleryItem>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.UploadDate, opt => opt.Ignore())
            //    .ForMember(dest => dest.IsVisible, opt => opt.MapFrom(src => true));

            //CreateMap<UpdateGalleryItemDto, GalleryItem>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            //// ===== News =====
            //CreateMap<News, NewsDto>();
            //CreateMap<News, NewsListItemDto>();

            //CreateMap<CreateNewsDto, Domain.Entities.News>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => false));

            //CreateMap<UpdateNewsDto, Domain.Entities.News>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            //// ===== Newsletter =====
            //CreateMap<Newsletter, NewsletterDto>();
            //CreateMap<Newsletter, NewsletterListItemDto>();

            //CreateMap<CreateNewsletterDto, Newsletter>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => NewsletterStatus.Draft))
            //    .ForMember(dest => dest.SentAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.SentByUserId, opt => opt.Ignore());

            //CreateMap<UpdateNewsletterDto, Newsletter>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            //// ===== EmailSubscriber =====
            //CreateMap<EmailSubscriber, SubscriberDto>();

            //CreateMap<SubscribeDto, EmailSubscriber>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.SubscribedAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false)) // نیاز به تأیید
            //    .ForMember(dest => dest.UnsubscribeToken, opt => opt.MapFrom(src => Guid.NewGuid().ToString()));

            // ===== ContactMessage =====
            CreateMap<ContactMessage, ContactMessageDto>().ReverseMap();

            // ===== User =====
            CreateMap<User, UserDto>().ReverseMap();

            // ===== LogEntry =====
            CreateMap<LogEntry, LogEntryDto>()
               .ForMember(dest => dest.ExceptionMessage, opt => opt.MapFrom(src => src.ExceptionMessage))
               .ForMember(dest => dest.StackTrace, opt => opt.MapFrom(src => src.StackTrace))
               .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId ?? "Anonymous"))
               .ForMember(dest => dest.HashedIp, opt => opt.MapFrom(src => src.HashedIp ?? "-"))
               .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method ?? "-"))
               .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.StatusCode ?? 0));
        }
    }
}