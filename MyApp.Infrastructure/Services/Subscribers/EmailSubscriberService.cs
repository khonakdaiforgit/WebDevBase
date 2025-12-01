using AutoMapper;
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
    private readonly ICurrentUserService _currentUser;

    public EmailSubscriberService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    public async Task ConfirmAsync(string token)
    {
        var subscriber = await _unitOfWork.EmailSubscribers.GetByUnsubscribeTokenAsync(token)
                ?? throw new NotFoundException("Invalid or expired confirmation token.");

        if (subscriber.IsActive)
            throw new ValidationException("Subscription already confirmed.");

        subscriber.IsActive = true;
        await _unitOfWork.EmailSubscribers.UpdateAsync(subscriber);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnsubscribeAsync(string emailOrToken)
    {
        var subscriber = await _unitOfWork.EmailSubscribers.GetByUnsubscribeTokenAsync(emailOrToken)
                 ?? throw new NotFoundException("Invalid unsubscribe token.");

        if (!subscriber.IsActive)
            throw new ValidationException("You are already unsubscribed.");

        subscriber.IsActive = false;
        await _unitOfWork.EmailSubscribers.UpdateAsync(subscriber);
        await _unitOfWork.SaveChangesAsync();
    }


    public async Task SubscribeAsync(string email)
    {
        var existing = await _unitOfWork.EmailSubscribers.GetByEmailAsync(email);
        if (existing != null)
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
            var callerId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
            var user = await _unitOfWork.Users.GetByIdAsync(callerId);

            var subscriber = new EmailSubscriber
            {
                Email = email,
                UnsubscribeToken = GenerateToken(),
                IsActive = false
            };
            await _unitOfWork.EmailSubscribers.AddAsync(subscriber);
        }
    }

    public async Task<List<string>> GetActiveEmailsAsync()
    {
        var restaurant = await _unitOfWork.Restaurants.GetMain();

        var callerId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        if (restaurant.OwnerUserId != callerId)
            throw new ForbiddenException("You are not the owner of this restaurant.");

        var subscribers = await _unitOfWork.EmailSubscribers.GetActiveByRestaurantAsync();
        return subscribers.Select(s => s.Email).ToList();
    }

    public async Task<PagedResult<SubscriberDto>> GetListAsync(int page = 1, int pageSize = 50)
    {
        var restaurant = await _unitOfWork.Restaurants.GetMain();

        var callerId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        if (restaurant.OwnerUserId != callerId)
            throw new ForbiddenException("You are not the owner of this restaurant.");

        var subscribers = await _unitOfWork.EmailSubscribers.GetActiveByRestaurantAsync();
        var dtos = _mapper.Map<List<SubscriberDto>>(subscribers);

        var total = dtos.Count;
        var paged = dtos.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<SubscriberDto>(paged, total, page, pageSize);
    }
}