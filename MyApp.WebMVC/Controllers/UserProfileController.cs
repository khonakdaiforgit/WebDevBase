using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.User;
using MyApp.WebMVC.Helper;
using MyApp.WebMVC.Models;
using Newtonsoft.Json;
using System.Text;

namespace MyApp.WebMVC.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public UserProfileController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper)
        {
            _httpClient = httpClientFactory.CreateClient("AuthApi");
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var userId = this.GetCurrentUserId();

            var response = await _httpClient.GetAsync($"user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();

                if (user?.WalletAddress == null)
                {
                    return RedirectToAction("UpdateWallet");
                }

                var responseUserProfile = await _httpClient.GetAsync($"logs/GetUserProfile/{userId}");
                var userProfile = await responseUserProfile.Content.ReadFromJsonAsync<UserProfileDto>();
                var viewModels = _mapper.Map<UserDashboardDataViewModel>(userProfile);
                return View(viewModels);
            }


            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateWallet()
        {
            var userId = this.GetCurrentUserId();

            var response = await _httpClient.GetAsync($"user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                var viewModels = _mapper.Map<UserProfileViewModel>(user);
                return View(viewModels);
            }
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> Affiliate()
        {
            var userId = this.GetCurrentUserId();

            var response = await _httpClient.GetAsync($"user/AffiliateData/{userId}");

            if (response.IsSuccessStatusCode)
            {

                var userAffiliateData = await response.Content.ReadFromJsonAsync<UserAffiliateDataDto>();
                var viewModels = _mapper.Map<UserAffiliateDataViewModel>(userAffiliateData);

                return View(viewModels);
            }
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> AffiliatePayment()
        {
            var userId = this.GetCurrentUserId();

            var response = await _httpClient.GetAsync($"user/AffiliatePayment/{userId}");

            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("Affiliate");
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWallet(UserProfileViewModel model)
        {

            var userId = this.GetCurrentUserId();

            var response = await _httpClient.GetAsync($"user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();

                var newUser = new UserUpdateDto()
                {
                    Id = user.Id,
                    WalletAddress = model.WalletAddress,
                    Balance = user.Balance,
                    PasswordHash = user.PasswordHash
                };

                var content = new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json");

                var response1 = await _httpClient.PutAsync($"user/{userId}", content);

                return Redirect(nameof(Index));
            }
            return NoContent();
        }
    }
}
