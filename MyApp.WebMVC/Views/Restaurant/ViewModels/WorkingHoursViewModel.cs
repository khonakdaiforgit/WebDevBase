using MyApp.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Views.Restaurant.ViewModels;

public class WorkingHoursViewModel
{
    public Guid RestaurantId { get; set; }

    // Sunday
    public TimeSpan? SundayOpen { get; set; } = new(09, 0, 0);
    public TimeSpan? SundayClose { get; set; } = new(22, 0, 0);
    public bool SundayClosed { get; set; }

    // Monday → Saturday (همه مشابه)
    public TimeSpan? MondayOpen { get; set; } = new(09, 0, 0);
    public TimeSpan? MondayClose { get; set; } = new(22, 0, 0);
    public bool MondayClosed { get; set; }

    public TimeSpan? TuesdayOpen { get; set; } = new(09, 0, 0);
    public TimeSpan? TuesdayClose { get; set; } = new(22, 0, 0);
    public bool TuesdayClosed { get; set; }

    public TimeSpan? WednesdayOpen { get; set; } = new(09, 0, 0);
    public TimeSpan? WednesdayClose { get; set; } = new(22, 0, 0);
    public bool WednesdayClosed { get; set; }

    public TimeSpan? ThursdayOpen { get; set; } = new(09, 0, 0);
    public TimeSpan? ThursdayClose { get; set; } = new(22, 0, 0);
    public bool ThursdayClosed { get; set; }

    public TimeSpan? FridayOpen { get; set; } = new(09, 0, 0);
    public TimeSpan? FridayClose { get; set; } = new(23, 0, 0);
    public bool FridayClosed { get; set; }

    public TimeSpan? SaturdayOpen { get; set; } = new(09, 0, 0);
    public TimeSpan? SaturdayClose { get; set; } = new(23, 0, 0);
    public bool SaturdayClosed { get; set; }

    // اگر WorkingHours در DTO داری، می‌تونی اینجا پر کنی
    public Dictionary<string, TimeRange>? Hours { get; set; }
}