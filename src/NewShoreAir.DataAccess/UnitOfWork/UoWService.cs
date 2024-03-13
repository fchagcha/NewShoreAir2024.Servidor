namespace NewShoreAir.DataAccess.UnitOfWork
{
    public class UoWService(NewShoreAirDbContext dbContext) : IUoWService
    {
        private volatile bool _disposedValue;
        private IDbContextTransaction _currentTransaction;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                    dbContext?.Dispose();

                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public bool ExisteTransaccionActiva()
        {
            return _currentTransaction != null;
        }

        public IExecutionStrategy CrearEstrategiaDeEjecucion()
        {
            return dbContext.Database.CreateExecutionStrategy();
        }
        public async Task<IDbContextTransaction> IniciarTransaccionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await dbContext.Database.BeginTransactionAsync();

            return _currentTransaction;
        }
        public async Task ConfirmarTransaccionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SalvarCambiosAsync(cancellationToken);
                await transaction.CommitAsync();
            }
            catch
            {
                await RevertirTransaccionAsync();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
        private async Task<int> SalvarCambiosAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
        private async Task RevertirTransaccionAsync()
        {
            try
            {
                await _currentTransaction?.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task IniciarTransaccionSimpleAsync()
        {
            _currentTransaction = await dbContext.Database.BeginTransactionAsync();
        }
        public async Task ConfirmarTransaccionSimpleAsync()
        {
            await _currentTransaction.CommitAsync();
        }
        public async Task RevertirTransaccionSimpleAsync()
        {
            await _currentTransaction.RollbackAsync();
        }
        public async Task<int> SalvarCambiosSimpleAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}