namespace NewShoreAir.DataAccess.Behaviours
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var validation =
                _validators
                .Select(x => x.ValidateAsync(context, cancellationToken));

            var validationResults = await
                Task.WhenAll(validation);

            var failures =
                validationResults
                .SelectMany(x => x.Errors)
                .Where(x => x != null);

            if (failures.Any())
                throw new CustomValidationException(failures);

            return await next();
        }
    }
}
