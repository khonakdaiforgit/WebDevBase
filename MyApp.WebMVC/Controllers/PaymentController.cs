using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace MyApp.WebMVC.Controllers
{
    
    public class PaymentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string? _apiKey;
        private readonly string? _ipnSecret;
        public PaymentController(
           IHttpClientFactory httpClientFactory,
           IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;

            _apiKey = configuration["NOWPayments:ApiKey"];
            _ipnSecret = configuration["NOWPayments:IpnSecret"];
        }

        [HttpGet]
        public async Task<IActionResult> StartPayment(string fileId,decimal price)
        {
            // پارامترها رو به دلخواه تنظیم کن
            string orderId = $"ORDER-{fileId}-{DateTime.UtcNow.Ticks}";
            decimal amount = price;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", _apiKey);

            var body = new
            {
                price_amount = amount,
                price_currency = "usd",
                pay_currency = "usdttrc20",
                order_id = orderId,
                order_description = "Pay for Product",
                ipn_callback_url = "http://cryptofiles.runasp.net/Payment/callback",
                is_fixed_rate = true, // نرخ ثابت
                is_fee_paid_by_user = true // کارمزد توسط شما پرداخت می‌شود
            };

            var jsonBody = JsonConvert.SerializeObject(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.nowpayments.io/v1/invoice", content);
            var result = await response.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(result);
            string payUrl = Convert.ToString(data.invoice_url);

            return Redirect(payUrl); // کاربر مستقیم به صفحه پرداخت منتقل میشه
        }

    }
}
