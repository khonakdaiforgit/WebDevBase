using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Domain.Dtos
{
    public class UserRevenueStats
    {
        public string UserId { get; set; }
        public decimal TotalRevenue { get; set; }
        public Dictionary<DateTime, decimal> RevenueByDay { get; set; } = new Dictionary<DateTime, decimal>();
    }
}
