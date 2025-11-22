// Domain/ValueObjects/WorkingHours.cs
using MyApp.Domain.Exceptions;

namespace MyApp.Domain.ValueObjects
{
    public class WorkingHours
    {
        // تغییر کلید از DayOfWeek به string
        public Dictionary<string, TimeRange> DailyHours { get; private set; } = new();

        private WorkingHours() { }

        public static WorkingHours Create(Dictionary<string, (TimeSpan open, TimeSpan close)> hours)
        {
            var workingHours = new WorkingHours(); // اینجا DailyHours قبلاً new شده
            foreach (var (dayName, (open, close)) in hours)
            {
                if (!IsValidDayName(dayName))
                    throw new DomainException($"Invalid day name: {dayName}");

                if (open >= close)
                    throw new DomainException($"Close time must be after open time on {dayName}");

                workingHours.DailyHours[dayName] = new TimeRange(open, close);
            }
            return workingHours;
        }
        public void UpdateDay(string dayName, TimeSpan open, TimeSpan close)
        {
            if (!IsValidDayName(dayName))
                throw new DomainException($"Invalid day name: {dayName}");

            if (open >= close)
                throw new DomainException("Close time must be after open time.");

            DailyHours[dayName] = new TimeRange(open, close);
        }

        public bool IsOpenNow()
        {
            var now = DateTime.Now;
            var todayEnglish = now.DayOfWeek.ToString(); // "Monday", "Tuesday", ...
            var currentTime = now.TimeOfDay;

            if (!DailyHours.TryGetValue(todayEnglish, out var range))
                return false;

            return currentTime >= range.Open && currentTime <= range.Close;
        }

        public string GetTodayHours()
        {
            var today = DateTime.Today.DayOfWeek.ToString();
            return DailyHours.TryGetValue(today, out var range)
                ? $"{range.Open:hh\\:mm} - {range.Close:hh\\:mm}"
                : "Closed";
        }

        private static bool IsValidDayName(string day) =>
            new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" }
            .Contains(day);
    }

    public record TimeRange(TimeSpan Open, TimeSpan Close);
}