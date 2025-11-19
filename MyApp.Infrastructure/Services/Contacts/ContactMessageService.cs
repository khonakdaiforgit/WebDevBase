using MyApp.Application.Abstractions.Contacts;
using MyApp.Application.Abstractions.Contacts.Dtos;
using MyApp.Application.Abstractions.Users;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Common.Exceptions;

namespace MyApp.Infrastructure.Services.Contacts
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public ContactMessageService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Submit a new contact message from the public website
        /// </summary>
        public async Task<Guid> SubmitAsync(string name, string email, string message, Guid? restaurantId = null)
        {
            name = name.Trim();
            email = email.Trim().ToLowerInvariant();
            message = message.Trim();

            if (string.IsNullOrWhiteSpace(name)) throw new BadRequestException("Name is required.");
            if (string.IsNullOrWhiteSpace(email)) throw new BadRequestException("Email is required.");
            if (string.IsNullOrWhiteSpace(message)) throw new BadRequestException("Message is required.");

            // In single-restaurant mode: use the only existing restaurant if not provided
            if (!restaurantId.HasValue)
            {
                restaurantId = await GetSystemRestaurantIdAsync();
            }

            var contactMessage = new ContactMessage
            {
                Name = name,
                Email = email,
                Message = message,
                RestaurantId = restaurantId.Value,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _unitOfWork.ContactMessages.AddAsync(contactMessage);
            await _unitOfWork.SaveChangesAsync();

            // TODO: Fire domain event or background job to send notification email to restaurant owner

            return contactMessage.Id;
        }

        /// <summary>
        /// Mark a contact message as read (admin panel)
        /// </summary>
        public async Task MarkAsReadAsync(Guid messageId, Guid callerUserId)
        {
            var message = await _unitOfWork.ContactMessages.GetByIdAsync(messageId)
                ?? throw new NotFoundException("Contact message not found.");

            await ValidateRestaurantAccessAsync(message.RestaurantId, callerUserId);

            if (message.IsRead)
                return; // already read

            message.IsRead = true;

            await _unitOfWork.ContactMessages.UpdateAsync(message);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Get paginated list of contact messages for the admin panel
        /// </summary>
        public async Task<PagedResult<ContactMessageDto>> GetListAsync(
            Guid? restaurantId = null,
            bool? onlyUnread = null,
            int page = 1,
            int pageSize = 20)
        {
            if (!restaurantId.HasValue)
            {
                restaurantId = await GetCurrentUserRestaurantIdAsync();
            }
            else
            {
                await ValidateRestaurantAccessAsync(restaurantId.Value, _currentUser.UserId.Value);
            }

            var pagedResult = await _unitOfWork.ContactMessages.GetPagedAsync(
                restaurantId: restaurantId.Value,
                onlyUnread: onlyUnread,
                page: page,
                pageSize: pageSize);

            var dtos = pagedResult.Items.Select(m => new ContactMessageDto(
                m.Id,
                m.Name,
                m.Email,
                m.Message,
                m.SentAt,
                m.IsRead
            )).ToList();

            return new PagedResult<ContactMessageDto>(
                dtos,
                pagedResult.TotalCount,
                page,
                pageSize);
        }

        // Private helpers
        private async Task ValidateRestaurantAccessAsync(Guid restaurantId, Guid userId)
        {
            var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId)
                ?? throw new NotFoundException("Restaurant not found.");

            // Single-restaurant system: all authenticated users have access
            // Future enhancement: check ownership or role
            return;
        }

        private async Task<Guid> GetCurrentUserRestaurantIdAsync()
        {
            var restaurants = await _unitOfWork.Restaurants.GetAllAsync();
            return restaurants.FirstOrDefault()?.Id
                ?? throw new InvalidOperationException("No restaurant found in the system.");
        }

        private async Task<Guid> GetSystemRestaurantIdAsync()
        {
            var restaurants = await _unitOfWork.Restaurants.GetAllAsync();
            return restaurants.FirstOrDefault()?.Id
                ?? throw new InvalidOperationException("No restaurant configured. Contact form is unavailable.");
        }
    }
}
