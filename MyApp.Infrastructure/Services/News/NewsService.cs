using MyApp.Application.Abstractions.News;
using MyApp.Application.Abstractions.News.Dtos;
using MyApp.Application.Common;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Common.Exceptions;


namespace MyApp.Infrastructure.Services.News
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateAsync(CreateNewsDto dto)
        {

            var news = new Domain.Entities.News
            {
                Title = dto.Title.Trim(),
                Content = dto.Content.Trim(),
                ImageUrl = dto.ImageUrl,
                PublishDate = DateTime.UtcNow,
                IsPublished = false
            };

            await _unitOfWork.News.AddAsync(news);
            await _unitOfWork.SaveChangesAsync();

            return news.Id;
        }

        public async Task UpdateAsync(UpdateNewsDto dto)
        {
            var news = await _unitOfWork.News.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException("News not found.");


            if (dto.Title is not null) news.Title = dto.Title.Trim();
            if (dto.Content is not null) news.Content = dto.Content.Trim();
            if (dto.ImageUrl is not null) news.ImageUrl = dto.ImageUrl;
            if (dto.IsPublished.HasValue)
            {
                news.IsPublished = dto.IsPublished.Value;
                if (news.IsPublished)
                    news.PublishDate = DateTime.UtcNow;
            }

            await _unitOfWork.News.UpdateAsync(news);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid newsId)
        {
            var news = await _unitOfWork.News.GetByIdAsync(newsId)
                ?? throw new NotFoundException("News not found.");


            await _unitOfWork.News.DeleteAsync(newsId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task PublishAsync(Guid newsId)
        {
            var news = await _unitOfWork.News.GetByIdAsync(newsId)
                ?? throw new NotFoundException("News not found.");


            news.Publish();
            news.PublishDate = DateTime.UtcNow;

            await _unitOfWork.News.UpdateAsync(news);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnpublishAsync(Guid newsId)
        {
            var news = await _unitOfWork.News.GetByIdAsync(newsId)
                ?? throw new NotFoundException("News not found.");


            news.Unpublish();

            await _unitOfWork.News.UpdateAsync(news);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<NewsDto?> GetAsync(Guid newsId)
        {
            var news = await _unitOfWork.News.GetByIdAsync(newsId);
            if (news is null) return null;

            return new NewsDto(
                news.Id,
                news.Title,
                news.Content,
                news.ImageUrl,
                news.PublishDate,
                news.IsPublished);
        }

        public async Task<PagedResult<NewsListItemDto>> GetListAsync(int page = 1, int pageSize = 20)
        {


            var paged = await _unitOfWork.News.GetPagedByRestaurantAsync(
                onlyPublished: null, // return both published & draft for admin panel
                page,
                pageSize);

            var dtos = paged.Items.Select(n => new NewsListItemDto(
                n.Id,
                n.Title,
                n.ImageUrl,
                n.Content,
                n.PublishDate,
                n.IsPublished)).ToList();

            return new PagedResult<NewsListItemDto>(dtos, paged.TotalCount, page, pageSize);
        }
    }
}
