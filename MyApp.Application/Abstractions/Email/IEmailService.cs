using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.Abstractions.Email
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string to, string restaurantName, string confirmLink, CancellationToken ct = default);
        Task SendNewsletterAsync(string to, string subject, string htmlContent, CancellationToken ct = default);
    }
}
