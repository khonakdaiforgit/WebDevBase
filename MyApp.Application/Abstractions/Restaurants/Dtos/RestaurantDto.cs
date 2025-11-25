using MyApp.Domain.ValueObjects;

namespace MyApp.Application.Abstractions.Restaurants.Dtos;
public record RestaurantDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Address { get; init; } = null!;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string Phone { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? LogoUrl { get; init; }
    public Dictionary<string, TimeRangeDto> WorkingHours { get; init; } = new();

    // ←←← این دو تا جادویی رو اضافه کن
    public bool IsOpenNow => CalculateIsOpenNow();
    public string TodayHoursDisplay => GetTodayHoursDisplay();

    private bool CalculateIsOpenNow()
    {
        var now = DateTime.Now;
        var today = now.DayOfWeek.ToString();
        var timeNow = now.TimeOfDay;

        if (!WorkingHours.TryGetValue(today, out var range))
            return false;

        return timeNow >= range.Open && timeNow <= range.Close;
    }

    private string GetTodayHoursDisplay()
    {
        var today = DateTime.Today.DayOfWeek.ToString();
        return WorkingHours.TryGetValue(today, out var range)
            ? $"{range.Open:hh\\:mm} - {range.Close:hh\\:mm}"
            : "Closed";
    }
}

public record TimeRangeDto(TimeSpan Open, TimeSpan Close);