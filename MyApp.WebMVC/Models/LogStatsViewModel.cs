namespace MyApp.WebMVC.Models
{
    public class LogStatsViewModel
    {
        public Dictionary<string, int> VisitsByDay { get; set; } = new();
        public Dictionary<string, int> RegistrationsByDay { get; set; } = new();
        public Dictionary<string, int> FileSaleViewsByDay { get; set; } = new();
        public Dictionary<string, int> SuccessfulSalesByDay { get; set; } = new();
    }
}
