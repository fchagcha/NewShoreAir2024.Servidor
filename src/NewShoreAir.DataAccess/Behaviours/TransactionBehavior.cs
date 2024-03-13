namespace NewShoreAir.DataAccess.Behaviours
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IUoWService uowServices;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

        public TransactionBehavior(
            IProvider provider,
            ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            uowServices = provider.ObtenerUnitOfWork<IUoWService>();
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);

            if (uowServices.ExisteTransaccionActiva())
                return await next();

            var strategy = uowServices.CrearEstrategiaDeEjecucion();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await uowServices.IniciarTransaccionAsync();

                using (_logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                {
                    response = await next();

                    await uowServices.ConfirmarTransaccionAsync(transaction, cancellationToken);
                }
            });

            return response;
        }
    }
}