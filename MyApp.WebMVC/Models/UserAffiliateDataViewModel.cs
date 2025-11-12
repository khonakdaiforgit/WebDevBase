namespace MyApp.WebMVC.Models
{
    public class UserAffiliateDataViewModel
    {
        public string? UserReferralCode { get; set; }
        public int RegisteredUserCount { get; set; }
        public int RelatedProductCount { get; set; }
        public int RelatedProductSealsCount { get; set; }
        public decimal RelatedProductSealsAmount { get; set; }

        public DateTimeOffset  LastAffiliatePayment { get; set; }
    }
}
