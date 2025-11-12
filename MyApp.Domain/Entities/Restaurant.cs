using MyApp.Domain.Interfaces.Common;
using MyApp.Domain.ValueObjects;

namespace MyApp.Domain.Entities
{
    public class Restaurant : IHasId<Guid>
    {
        public Guid Id { get;  set; } = Guid.NewGuid();
        public string Name { get;  set; }
        public string Address { get;  set; }
        public string Phone { get;  set; }
        public string Email { get;  set; }
        public string LogoUrl { get;  set; }
        public WorkingHours WorkingHours { get;  set; }

        // رفتار دامنه (Domain Behavior)
        public void UpdateLogo(string url) => LogoUrl = url;
        public void UpdateHours(WorkingHours hours) => WorkingHours = hours;
    }
}
