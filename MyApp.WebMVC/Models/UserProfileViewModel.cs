namespace MyApp.WebMVC.Models
{
    public class UserProfileViewModel
    {
        public Guid Id { get; set; }
        public string? PasswordHash { get; set; }
        public string? WalletAddress { get; set; }
        public decimal? Balance { get; set; }
        public string? ReferralCode { get; set; } // optional
        public string? RegisteredWithReferralCode { get; set; } // optional
    }
}
