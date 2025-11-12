using MyApp.Domain.Exceptions;

namespace MyApp.Domain.ValueObjects
{
    // Core/ValueObjects/WorkingHours.cs
    public class WorkingHours
    {
        public Dictionary<DayOfWeek, TimeRange> DailyHours { get; private set; } = new();

        private WorkingHours() { }

        public static WorkingHours Create(Dictionary<DayOfWeek, (TimeSpan open, TimeSpan close)> hours)
        {
            var workingHours = new WorkingHours();

            foreach (var (day, (open, close)) in hours)
            {
                if (open >= close)
                    throw new DomainException($"Close time ({close}) cannot be before or equal to open time ({open}) on {day}");

                workingHours.DailyHours[day] = new TimeRange(open, close);
            }

            return workingHours;
        }

        public void UpdateDay(DayOfWeek day, TimeSpan open, TimeSpan close)
        {
            if (open >= close)
                throw new DomainException("Close time must be after open time.");

            DailyHours[day] = new TimeRange(open, close);
        }

        public string GetFormattedHours(DayOfWeek day)
        {
            if (!DailyHours.ContainsKey(day)) return "Closed";
            var range = DailyHours[day];
            return $"{range.Open:hh\\:mm} - {range.Close:hh\\:mm}";
        }
    }

    public record TimeRange(TimeSpan Open, TimeSpan Close);
}
