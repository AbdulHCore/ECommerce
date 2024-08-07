using FluentValidation.Results;
using System.ComponentModel;

namespace Ordering.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public ValidationException() : base ("One or more validation error(s) occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this ()
        {
            Errors = failures.GroupBy(e=> e.PropertyName, e=>e.ErrorMessage)
                .ToDictionary(failure =>  failure.Key, failure => failure.ToArray());
        }
    }
}
