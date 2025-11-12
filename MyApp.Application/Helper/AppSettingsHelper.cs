
namespace MyApp.Application.Helper
{
    public class AppSettingsHelper
    {
        public static bool local = false;
        public static int refreshTokenExpireAfterMints = 7 * 24 * 60;
        public static double accessTokenExpireAfterMints = 15;

        public const decimal SiteCommissionRate = 0.02m; // 2%
        public const decimal MinimumPayoutAmount = 25m;
        public const decimal UserAffiliateRate = 0.5m;
        public static string GetMvcAddress()
        {
            if (local)
                return "https://localhost:5002/";
            else
                //return "http://cryptofile.runasp.net/";
            return "https://cryptofileproject.onrender.com/";

        }

        public static string GetApiAddress()
        {
            if (local)
                return "https://localhost:5000/";
            else
                return "http://cryptofileapi.runasp.net/";
            //return "https://linksafe-hx12.onrender.com/";

        }
    }
}
