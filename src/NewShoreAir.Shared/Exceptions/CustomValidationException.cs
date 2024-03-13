using FluentValidation.Results;

namespace NewShoreAir.Shared.Exceptions
{
    public class CustomValidationException : ApplicationException
    {
        public IDictionary<string, string[]> Errors { get; }

        public CustomValidationException() : base("Se presentaron uno o mas errores de validación")
        {
            Errors = new Dictionary<string, string[]>();
        }
        public CustomValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

        }
    }
}