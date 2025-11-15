using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Infrastructure.Common.Exceptions
{
    [Serializable]
    public class ValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<KeyValuePair<string, string[]>> failures)
            : this()
        {
            Errors = failures.ToDictionary(f => f.Key, f => f.Value);
        }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string paramName, string message)
            : this()
        {
            Errors = new Dictionary<string, string[]>
            {
                [paramName] = new[] { message }
            };
        }

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Errors = (Dictionary<string, string[]>)info.GetValue("Errors", typeof(Dictionary<string, string[]>))!;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Errors", Errors);
        }
    }
}
