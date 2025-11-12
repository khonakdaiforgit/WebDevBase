using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs;
using MyApp.WebMVC.Models;

namespace MyApp.WebMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public DashboardController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper)
        {
            _httpClient = httpClientFactory.CreateClient("AuthApi");
            _mapper = mapper;
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logs(LogFilterDto filter = null)
        {
            try
            {
                // اگر فیلتر null باشه (مثلاً در GET اولیه)، یه نمونه پیش‌فرض بساز
                filter ??= new LogFilterDto { PageNumber = 1, PageSize = 10 };

                // اطمینان از مقادیر معتبر برای PageNumber و PageSize
                filter.PageNumber = Math.Max(1, filter.PageNumber);
                filter.PageSize = Math.Max(1, filter.PageSize);

                //// اگر درخواست GET باشه و فیلتر خالی باشه، فقط ویو رو برگردون
                //if (HttpContext.Request.Method == "GET" && 
                //    filter.UserId == null && 
                //    filter.Level == null && 
                //    filter.Project == null && 
                //    filter.SearchTerm == null &&
                //    filter.PageNumber == 1 &&
                //    !filter.StartDate.HasValue && 
                //    !filter.EndDate.HasValue)
                //{
                //    ViewBag.PagedResult = new PagedLogResultDto();
                //    return View(filter);
                //}

                var response = await _httpClient.PostAsJsonAsync("logs/filterWithPaging", filter);
                response.EnsureSuccessStatusCode();
                var pagedResult = await response.Content.ReadFromJsonAsync<PagedLogResultDto>();
                ViewBag.PagedResult = pagedResult ?? new PagedLogResultDto();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to load logs: {ex.Message}";
                ViewBag.PagedResult = new PagedLogResultDto();
            }
            return View(filter);
        }

     
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Index(LogFilterDto filter)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("logs/stats", filter);
                response.EnsureSuccessStatusCode();
                var stats = await response.Content.ReadFromJsonAsync<LogStatsDto>();

                var log = new LogStatsViewModel();

                log.SuccessfulSalesByDay = stats.SuccessfulSalesByDay
                    .ToDictionary(
                        x => x.Key.Date.ToShortDateString(),  
                        x => x.Value);

                log.RegistrationsByDay = stats.RegistrationsByDay
                    .ToDictionary(
                    x => x.Key.Date.ToShortDateString(),
                    x => x.Value);

                log.FileSaleViewsByDay = stats.FileSaleViewsByDay
                    .ToDictionary(
                    x => x.Key.Date.ToShortDateString(),
                    x => x.Value);

                log.VisitsByDay = stats.VisitsByDay
                    .ToDictionary(
                    x => x.Key.Date.ToShortDateString(),
                    x => x.Value);

                ViewBag.Stats = log ?? new LogStatsViewModel();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to load stats: {ex.Message}";
                ViewBag.Stats = new LogStatsDto();
            }
            return View(filter);
        }

       
    }
}
