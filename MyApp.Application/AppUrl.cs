using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application
{
    public static class AppUrl
    {
        public static string GetApiUrl()
        {
            var url = "https://localhost:5100/";
#if !DEBUG
            url= "https://localhost:5001/";
#endif
            return url;
        }

        public static string GetMVCUrl()
        {
            var url = "https://localhost:5102/";
#if !DEBUG
            url= "https://localhost:5002/";
#endif
            return url;
        }
    }
}
