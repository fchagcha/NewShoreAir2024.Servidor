namespace NewShoreAir.DataAccess.UnitOfWork
{
    public class UnitOfWorkService(NewShoreAirDbContext dbContext) : IUnitOfWorkService
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

        public async Task ConfirmarTransaccionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SalvarCambiosAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await RevertirTransaccion();
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

        private async Task<int> SalvarCambiosAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
        private async Task RevertirTransaccion()
        {
            try
            {
                if (_currentTransaction is not null)
                    await _currentTransaction.RollbackAsync();
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
    }
}