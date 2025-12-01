using MyApp.Application.Abstractions.Email;
using MyApp.Application.Abstractions.Logging;
using MyApp.Application.Abstractions.Logging.Dtos;
using MyApp.Application.Abstractions.Newsletters;
using MyApp.Application.Abstractions.Newsletters.Dtos;
using MyApp.Application.Abstractions.Users;
using MyApp.Application.Common;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Common.Exceptions;


namespace MyApp.Infrastructure.Services.Newsletter
{
    public class NewsletterService : INewsletterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ILogService _logService;
        private readonly ICurrentUserService _currentUser;

        public NewsletterService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            ILogService logService,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _logService = logService;
            _currentUser = currentUser;
        }


        public async Task<NewsletterDto?> GetAsync(Guid newsletterId)
        {
            var newsletter = await _unitOfWork.Newsletters.GetByIdAsync(newsletterId);
            if (newsletter == null) return null;

            return new NewsletterDto(
                Id: newsletter.Id,
                Subject: newsletter.Subject,
                Content: newsletter.Content,
                SentAt: newsletter.SentAt,
                SentByUserId: newsletter.SentByUserId,
                Status: newsletter.Status
            );
        }

        private async Task LogActionAsync(string message, Guid userId, Guid newsletterId, string level = "Info", int statusCode = 200)
        {
            await _logService.LogAsync(new LogEntryDto(
                Id: Guid.NewGuid(),
                Timestamp: DateTime.UtcNow,
                Level: level,
                Message: $"{message} (NewsletterId: {newsletterId})",
                ExceptionMessage: null,
                StackTrace: null,
                UserId: userId.ToString(),
                HashedIp: null,
                Method: "POST",
                Path: $"/newsletters/{newsletterId}",
                StatusCode: statusCode,
                Project: "NewsletterService"
            ));
        }

        public async Task<Guid> CreateAsync(string subject, string content)
        {

            var newsletter = new Domain.Entities.Newsletter
            {
                Id = Guid.NewGuid(),
                Subject = subject,
                Content = content,
                Status = NewsletterStatus.Draft
            };

            await _unitOfWork.Newsletters.AddAsync(newsletter);
            await _unitOfWork.SaveChangesAsync();

            return newsletter.Id;
        }

        public async Task SendNowAsync(Guid newsletterId)
        {
            var newsletter = await _unitOfWork.Newsletters.GetByIdAsync(newsletterId);
            if (newsletter == null)
                throw new NotFoundException("Newsletter not found.");

            if (newsletter.Status == NewsletterStatus.Sent)
                throw new BadRequestException("Newsletter already sent.");

            var subscribers = await _unitOfWork.EmailSubscribers.GetActiveByRestaurantAsync();


            var tasks = subscribers.Select(email =>
            _emailService.SendAsync(
                to: email.Email,
                subject: newsletter.Subject,
                htmlBody: newsletter.Content // فرض می‌کنیم محتوا قبلاً HTML امن هست
            ));

            await Task.WhenAll(tasks);
            newsletter.Send(_currentUser.UserId.Value);

            await _unitOfWork.Newsletters.UpdateAsync(newsletter);
            await _unitOfWork.SaveChangesAsync();

            await LogActionAsync($"Newsletter sent to {subscribers.Count} subscribers", _currentUser.UserId.Value, newsletterId, "Info", 200);
        }

        public async Task<PagedResult<NewsletterListItemDto>> GetListAsync(int page = 1, int pageSize = 20)
        {
            var paged = await _unitOfWork.Newsletters.GetPagedAsync(
                      page: page,
                      pageSize: pageSize);

            var items = paged.Items.Select(n => new NewsletterListItemDto(
                Id: n.Id,
                Subject: n.Subject,
                SentAt: n.SentAt,
                Status: n.Status
            )).ToList();

            return new PagedResult<NewsletterListItemDto>(items, paged.TotalCount, page, pageSize);
        }

        public async Task DeleteAsync(Guid id)
        {
            var newsletter = await _unitOfWork.Newsletters.GetByIdAsync(id);
            if (newsletter == null)
                throw new NotFoundException("Newsletter not found.");

            await _unitOfWork.Newsletters.DeleteAsync(newsletter.Id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, string subject, string content)
        {
            var newsletter = await _unitOfWork.Newsletters.GetByIdAsync(id);
            if (newsletter == null)
                throw new NotFoundException("Newsletter not found.");

         
            if (subject != null) newsletter.Subject = subject;
            if (content != null) newsletter.Content = content;

            await _unitOfWork.Newsletters.UpdateAsync(newsletter);
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
