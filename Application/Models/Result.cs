using System.Linq;
using System.Collections.Generic;

namespace Tada.Application.Models
{
    public class Result
    {
        internal Result(bool succeeded, object data, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Data = data;
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; set; }

        public string[] Errors { get; set; }

        public object Data { get; set; }

        public static Result Success<T>(T data)
        {
            return new Result(true, data, new string[] { });
        }

        public static Result Success()
        {
            return new Result(true, null, new string[] { });
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, new object(), errors);
        }
    }
}
