using FluentAssertions;
using Mongo2Go;
using MongoDB.Driver;
using MyApp.Domain.Entities;
using MyApp.Infrastructure;
using MyApp.Infrastructure.Repositories;
using Xunit;

namespace MyApp.Tests.Infrastructure.Repositories
{
    public class ContactMessageRepositoryTests : IDisposable
    {
        private readonly MongoDbRunner _runner;
        private readonly IMongoClient _client;
        private readonly MongoDbSettings _settings;
        private readonly ContactMessageRepository _repository;
        private readonly IMongoCollection<ContactMessage> _collection;

        public ContactMessageRepositoryTests()
        {
            // 1. راه‌اندازی MongoDB در حافظه
            _runner = MongoDbRunner.Start();

            _client = new MongoClient(_runner.ConnectionString);

            _settings = new MongoDbSettings
            {
                DatabaseName = "TestContactDb"
            };

            // 2. ساخت ریپازیتوری
            _repository = new ContactMessageRepository(_client, _settings);

            // 3. دسترسی مستقیم به کالکشن برای تست (Assert)
            _collection = _client
                .GetDatabase(_settings.DatabaseName)
                .GetCollection<ContactMessage>("ContactMessages");
        }

        [Fact]
        public async Task AddAsync_ShouldInsertEntity_WithGeneratedIdAndSentAt()
        {
            // Arrange
            var message = new ContactMessage
            {
                // Id و SentAt توسط سازنده تنظیم می‌شن
                Name = "علی رضایی",
                Email = "ali@example.com",
                Message = "سلام، سایت خوبی دارید!"
            };

            // Act
            await _repository.AddAsync(message);

            // Assert
            var saved = await _collection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();
            saved.Should().NotBeNull();
            saved.Name.Should().Be("علی رضایی");
            saved.Email.Should().Be("ali@example.com");
            saved.Message.Should().Be("سلام، سایت خوبی دارید!");
            saved.IsRead.Should().BeFalse();
            saved.SentAt.Should().BeCloseTo(message.SentAt, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task GetAsync_ShouldReturnEntity_WhenIdExists()
        {
            // Arrange
            var message = new ContactMessage
            {
                Name = "تست",
                Email = "test@example.com",
                Message = "تست"
            };
            await _collection.InsertOneAsync(message);

            // Act
            var result = await _repository.GetAsync(message.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(message.Id);
            result.Name.Should().Be("تست");
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Act
            var result = await _repository.GetAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllMessages()
        {
            // Arrange
            var msg1 = new ContactMessage { Name = "اول", Email = "1@example.com", Message = "1" };
            var msg2 = new ContactMessage { Name = "دوم", Email = "2@example.com", Message = "2" };
            await _collection.InsertManyAsync(new[] { msg1, msg2 });

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(x => x.Id == msg1.Id);
            result.Should().Contain(x => x.Id == msg2.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReplaceEntireDocument()
        {
            // Arrange
            var original = new ContactMessage { Name = "قدیمی", Email = "old@example.com", Message = "قدیمی" };
            await _collection.InsertOneAsync(original);

            var updated = new ContactMessage
            {
                Id = original.Id,
                Name = "جدید",
                Email = "new@example.com",
                Message = "جدید"
            };

            // Act
            await _repository.UpdateAsync(updated);

            // Assert
            var saved = await _collection.Find(x => x.Id == original.Id).FirstOrDefaultAsync();
            saved.Name.Should().Be("جدید");
            saved.Email.Should().Be("new@example.com");
            saved.Message.Should().Be("جدید");
            saved.SentAt.Should().Be(original.SentAt); // تاریخ ارسال تغییر نمی‌کنه
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            // Arrange
            var message = new ContactMessage { Name = "حذف", Email = "delete@example.com", Message = "حذف" };
            await _collection.InsertOneAsync(message);

            // Act
            await _repository.DeleteAsync(message.Id);

            // Assert
            var exists = await _collection.Find(x => x.Id == message.Id).AnyAsync();
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task GetUnreadAsync_ShouldReturnOnlyUnreadMessages_SortedBySentAtDescending()
        {
            // Arrange
            var readMessage = new ContactMessage
            {
                Name = "خوانده شده",
                Email = "read@example.com",
                Message = "خوانده شده",
                SentAt = DateTime.UtcNow.AddHours(-3)
            };
            readMessage.MarkAsRead(); // استفاده از متد دامنه

            var unreadOld = new ContactMessage
            {
                Name = "خوانده نشده قدیمی",
                Email = "old@example.com",
                Message = "قدیمی",
                SentAt = DateTime.UtcNow.AddHours(-2)
            };

            var unreadNew = new ContactMessage
            {
                Name = "خوانده نشده جدید",
                Email = "new@example.com",
                Message = "جدید",
                SentAt = DateTime.UtcNow.AddHours(-1)
            };

            await _collection.InsertManyAsync(new[] { readMessage, unreadOld, unreadNew });

            // Act
            var result = await _repository.GetUnreadAsync();

            // Assert
            result.Should().HaveCount(2);
            result[0].Id.Should().Be(unreadNew.Id); // جدیدترین اول
            result[1].Id.Should().Be(unreadOld.Id);
            result.Should().NotContain(x => x.IsRead);
        }

        [Fact]
        public async Task MarkAsReadAsync_ShouldSetIsReadToTrue_UsingFindOneAndUpdate()
        {
            // Arrange
            var message = new ContactMessage
            {
                Name = "تست خواندن",
                Email = "mark@example.com",
                Message = "این پیام باید خوانده شود"
            };
            await _collection.InsertOneAsync(message);

            // Act
            await _repository.MarkAsReadAsync(message.Id);

            // Assert
            var updated = await _collection.Find(x => x.Id == message.Id).FirstOrDefaultAsync();
            updated.IsRead.Should().BeTrue();
        }

        [Fact]
        public async Task MarkAsReadAsync_ShouldDoNothing_WhenIdNotFound()
        {
            // Act
            await _repository.MarkAsReadAsync(Guid.NewGuid());

            // Assert: نباید خطا بده
            // (در این پیاده‌سازی، MongoDB فقط UpdateCount = 0 برمی‌گردونه)
        }

        public void Dispose()
        {
            _runner?.Dispose();
            _client?.Cluster?.Dispose();
        }
    }
}