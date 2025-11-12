using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs;
using MyApp.WebMVC.Models;
using System.Text.Json;

namespace MyApp.WebMVC.Controllers;

[Authorize]
public class ContactMessageController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;

    public ContactMessageController(
        IHttpClientFactory httpClientFactory, 
        IMapper mapper)
    {
        _httpClient = httpClientFactory.CreateClient("AuthApi");
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new ContactMessageCreateViewModel
        {
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactMessageCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var dto = _mapper.Map<ContactMessageCreateDto>(model);
            var response = await _httpClient.PostAsJsonAsync("ContactMessage", dto);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseString);
                var reportNumber = doc.RootElement.GetProperty("reportNumber").GetString();

                TempData["SuccessMessage"] = $"Your message has been submitted successfully. Report number: {reportNumber}";
                return RedirectToAction("Create");
            }

            TempData["ErrorMessage"] = "An error occurred while submitting your message. Please try again.";
            return View(model);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> UserContactMessages(int pageNumber = 1, int pageSize = 10, bool onlyUnread = false)
    {
        try
        {
            var query = $"?pageNumber={pageNumber}&pageSize={pageSize}&onlyUnread={onlyUnread}";
            var response = await _httpClient.GetAsync($"ContactMessage{query}");

            if (response.IsSuccessStatusCode)
            {
                var pagedResult = await response.Content.ReadFromJsonAsync<PagedResultDto<ContactMessageDto>>();
                var model = new ContactMessageListViewModel
                {
                    Messages = _mapper.Map<List<ContactMessageViewModel>>(pagedResult.Items),
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageIndex,
                    PageSize = pagedResult.PageSize,
                    OnlyUnread = onlyUnread
                };
                return View(model);
            }

            TempData["ErrorMessage"] = "Failed to load contact messages.";
            return View(new ContactMessageListViewModel { Messages = new List<ContactMessageViewModel>() });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred while loading contact messages.";
            return View(new ContactMessageListViewModel { Messages = new List<ContactMessageViewModel>() });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ContactMessagesDetails(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"ContactMessage/{id}");

            if (response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadFromJsonAsync<ContactMessageDto>();
                var model = _mapper.Map<ContactMessageViewModel>(message);

                // Mark as read
                if (!model.IsRead)
                {
                    var updateDto = new ContactMessageUpdateDto { Id = id, IsRead = true };
                    await _httpClient.PutAsJsonAsync($"ContactMessage/{id}", updateDto);
                }

                return View(model);
            }

            TempData["ErrorMessage"] = "Message not found.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the message.";
            return RedirectToAction("Index");
        }
    }
}