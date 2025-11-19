using MyApp.Application.Abstractions.Authorization;
using MyApp.Application.Abstractions.Contacts;
using MyApp.Application.Abstractions.Email;
using MyApp.Application.Abstractions.Galleries;
using MyApp.Application.Abstractions.Logging;
using MyApp.Application.Abstractions.Menus;
using MyApp.Application.Abstractions.News;
using MyApp.Application.Abstractions.Newsletters;
using MyApp.Application.Abstractions.Restaurants;
using MyApp.Application.Abstractions.Subscribers;
using MyApp.Application.Abstractions.Users;
using MyApp.Infrastructure.Services.Auth;
using MyApp.Infrastructure.Services.Authorization;
using MyApp.Infrastructure.Services.Contacts;
using MyApp.Infrastructure.Services.Email;
using MyApp.Infrastructure.Services.Galleries;
using MyApp.Infrastructure.Services.Logging;
using MyApp.Infrastructure.Services.Menus;
using MyApp.Infrastructure.Services.News;
using MyApp.Infrastructure.Services.Newsletter;
using MyApp.Infrastructure.Services.Restaurant;
using MyApp.Infrastructure.Services.Subscribers;
using MyApp.Infrastructure.Services.Users;

namespace MyApp.WebAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IEmailSubscriberService, EmailSubscriberService>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<INewsletterService, NewsletterService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IGalleryService, GalleryService>();
            services.AddScoped<IContactMessageService, ContactMessageService>();

            return services;
        }
    }
}
