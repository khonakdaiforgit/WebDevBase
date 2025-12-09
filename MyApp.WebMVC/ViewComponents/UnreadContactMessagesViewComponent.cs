using Microsoft.AspNetCore.Mvc;
using MyApp.WebMVC.Extensions;
using System.Text.Json;

namespace MyApp.WebMVC.ViewComponents
{
    public class UnreadContactMessagesViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UnreadContactMessagesViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient")
                                           .WithJwt(HttpContext);

            try
            {
                var response = await client.GetAsync("api/contact-messages/unread-count");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var count = JsonSerializer.Deserialize<int>(json);
                    return View(count);
                }
            }
            catch { /* ignore */ }

            return View(0);
        }
    }
}
