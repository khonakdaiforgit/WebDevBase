

namespace MyApp.Domain.Dtos
{
    public class LogStats
    {
        public Dictionary<DateTime, int> VisitsByDay { get; set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> RegistrationsByDay { get; set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> FileSaleViewsByDay { get; set; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> SuccessfulSalesByDay { get; set; } = new Dictionary<DateTime, int>();
    }
}
