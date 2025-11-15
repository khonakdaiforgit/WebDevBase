// src/Application/Services/EmailSubscriberService.cs
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyApp.Application.Abstractions.Email;
using MyApp.Application.Abstractions.Subscribers;
using MyApp.Application.Abstractions.Subscribers.Dtos;
using MyApp.Application.Abstractions.Users;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Common.Exceptions;
using System.Security.Cryptography;

namespace MyApp.Infrastructure.Services.Subscribers;

public class EmailSubscriberService : IEmailSubscriberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ICurrentUserService _currentUser;
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;
    private readonly ILogger<EmailSubscriberService> _logger;

    public EmailSubscriberService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEmailService emailService,
        ICurrentUserService currentUser,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor,
        IOptions<AppSettings> appSettings,
        ILogger<EmailSubscriberService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _emailService = emailService;
        _currentUser = currentUser;
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    public async Task SubscribeAsync(SubscribeDto dto)
    {
        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(dto.RestaurantId)
            ?? throw new NotFoundException("Restaurant", dto.RestaurantId);

        var existing = await _unitOfWork.EmailSubscribers.GetByEmailAsync(dto.Email);
        if (existing != null && existing.RestaurantId == dto.RestaurantId)
        {
            if (existing.IsActive)
                throw new ValidationException("Email", "You are already subscribed to this restaurant.");

            // فعال‌سازی مجدد
            existing.IsActive = false;
            existing.UnsubscribeToken = GenerateToken();
            await _unitOfWork.EmailSubscribers.UpdateAsync(existing);
        }
        else if (existing != null)
        {
            throw new ValidationException("Email", "This email is already used for another restaurant.");
        }
        else
        {
            var subscriber = new EmailSubscriber
            {
                RestaurantId = dto.RestaurantId,
                Email = dto.Email,
                UnsubscribeToken = GenerateToken(),
                IsActive = false
            };
            await _unitOfWork.EmailSubscribers.AddAsync(subscriber);
        }

        await _unitOfWork.SaveChangesAsync();

        var token = existing?.UnsubscribeToken ?? (await _unitOfWork.EmailSubscribers.GetByEmailAsync(dto.Email))!.UnsubscribeToken;
        var confirmLink = GenerateLink("Confirm", token);
        await _emailService.SendConfirmationEmailAsync(dto.Email, restaurant.Name, confirmLink);

        _logger.LogInformation("Subscription request for {Email} to restaurant {RestaurantId}", dto.Email, dto.RestaurantId);
    }

    public async Task ConfirmAsync(ConfirmSubscriptionDto dto)
    {
        var subscriber = await _unitOfWork.EmailSubscribers.GetByUnsubscribeTokenAsync(dto.Token)
            ?? throw new NotFoundException("Invalid or expired confirmation token.");

        if (subscriber.IsActive)
            throw new ValidationException("Subscription already confirmed.");

        subscriber.IsActive = true;
        await _unitOfWork.EmailSubscribers.UpdateAsync(subscriber);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Subscription confirmed for {Email}", subscriber.Email);
    }

    public async Task UnsubscribeAsync(UnsubscribeDto dto)
    {
        var subscriber = await _unitOfWork.EmailSubscribers.GetByUnsubscribeTokenAsync(dto.Token)
            ?? throw new NotFoundException("Invalid unsubscribe token.");

        if (!subscriber.IsActive)
            throw new ValidationException("You are already unsubscribed.");

        subscriber.IsActive = false;
        await _unitOfWork.EmailSubscribers.UpdateAsync(subscriber);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Unsubscribed {Email} from restaurant {RestaurantId}", subscriber.Email, subscriber.RestaurantId);
    }

    public async Task<PagedResult<SubscriberDto>> GetActiveListAsync(Guid restaurantId, int page = 1, int pageSize = 50)
    {
        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId)
            ?? throw new NotFoundException("Restaurant", restaurantId);

        var callerId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        if (restaurant.OwnerUserId != callerId)
            throw new ForbiddenException("You are not the owner of this restaurant.");

        var subscribers = await _unitOfWork.EmailSubscribers.GetActiveByRestaurantAsync(restaurantId);
        var dtos = _mapper.Map<List<SubscriberDto>>(subscribers);

        var total = dtos.Count;
        var paged = dtos.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<SubscriberDto>(paged, total, page, pageSize);
    }
    // --- متدهای کمکی ---
    private string GenerateLink(string action, string token)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            return _linkGenerator.GetUriByAction(
                httpContext: context,
                action: action,
                controller: "Subscribe",
                values: new { token },
                scheme: context.Request.Scheme,
                host: context.Request.Host
            )!;
        }

        return $"{_appSettings.BaseUrl.TrimEnd('/')}/subscribe/{action.ToLower()}?token={token}";
    }

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
}