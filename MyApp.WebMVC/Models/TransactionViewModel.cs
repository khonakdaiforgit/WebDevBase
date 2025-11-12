using MyApp.Application.DTOs;
using MyApp.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.WebMVC.Models
{
    public class TransactionViewModel
    {
        public Guid Id { get; set; }
        public string? BuyerWallet { get; set; }
        public string SellerWallet { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal Price { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public TransactionStatus Status { get; set; }

        public Guid FileLinkId { get; set; } // ارتباط با فایل
        public string? CallBackJsonData { get; set; }
        public string? payoutId { get; set; }
        public string? payoutData { get; set; }

        public TransactionStatus PayoutStatus { get; set; } = TransactionStatus.Waiting;

        public DateTimeOffset? PayTime { get; set; }
    }
}
