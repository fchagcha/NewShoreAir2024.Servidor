using Microsoft.Extensions.Logging;

namespace NewShoreAir.DataAccess.Behaviours
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWorkService _unitOfWork;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

        public TransactionBehavior(
            IUnitOfWorkProvider unitOfWorkProvider,
            ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _unitOfWork = unitOfWorkProvider.ObtenerUnitOfWork<IUnitOfWorkService>();
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                var response = default(TResponse);

                if (_unitOfWork.ExisteTransaccionActiva())
                    return await next();

                var strategy = _unitOfWork.CrearEstrategiaDeEjecucion();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _unitOfWork.IniciarTransaccionAsync();

                    using (_logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                    {
                        response = await next();

                        await _unitOfWork.ConfirmarTransaccionAsync(transaction);
                    }
                });

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}