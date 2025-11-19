namespace MyApp.Infrastructure.Services.Email.Settings
{
    public class SmtpSettings
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string From { get; set; } = "no-reply@myapp.com";
        public bool EnableSsl { get; set; } = true;
        public string FromName { get; set; } = "Restaurant";
    }
}
