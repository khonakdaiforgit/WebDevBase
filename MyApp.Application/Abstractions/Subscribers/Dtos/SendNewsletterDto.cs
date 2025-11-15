using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.Abstractions.Subscribers.Dtos
{
    public record SendNewsletterDto(string Subject, string HtmlContent);
}
