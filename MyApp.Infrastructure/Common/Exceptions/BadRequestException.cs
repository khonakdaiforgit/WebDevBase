using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Infrastructure.Common.Exceptions
{
    /// <summary>
    /// وقتی درخواست کاربر از نظر منطقی اشتباهه (مثلاً ویرایش خبرنامه‌ای که قبلاً ارسال شده)
    /// از این اکسپشن استفاده می‌شه → HTTP 400
    /// </summary>
    [Serializable]
    public class BadRequestException : Exception
    {
        public BadRequestException()
            : base("Bad Request.") { }

        public BadRequestException(string message)
            : base(message) { }

        public BadRequestException(string message, Exception innerException)
            : base(message, innerException) { }

        // برای سناریوهای ساده مثل "Newsletter already sent"
        public BadRequestException(string action, string reason)
            : base($"{action}: {reason}") { }

        protected BadRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
