namespace NewShoreAir.DataAccess.UnitOfWork
{
    public class UoWCommand(NewShoreAirDbContext dbContext) : IUoWCommand
    {
        private volatile bool _disposedValue;

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

        public async Task<TEntidad> ObtenerAsync<TEntidad>(
            Expression<Func<TEntidad, bool>> predicado,
            CancellationToken cancellationToken = default) where TEntidad : class, IEntity
        {
            var entidad = dbContext.Set<TEntidad>().Local.AsQueryable().FirstOrDefault(predicado);

            if (entidad is null)
            {
                var query = dbContext.Set<TEntidad>().AsQueryable();

                entidad = await query.FirstOrDefaultAsync(predicado, cancellationToken);
            }

            return entidad;
        }
        public IQueryable<TEntidad> Filtrar<TEntidad>(
            Expression<Func<TEntidad, bool>> predicado = null,
            CancellationToken cancellationToken = default) where TEntidad : class, IEntity
        {
            var query = dbContext.Set<TEntidad>().AsQueryable();

            if (predicado is not null)
                query = query.Where(predicado);

            return query;
        }
        public async Task<List<TEntidad>> ListarAsync<TEntidad>(
            Expression<Func<TEntidad, bool>> predicado = null,
            CancellationToken cancellationToken = default) where TEntidad : class, IEntity
        {
            var query = dbContext.Set<TEntidad>().AsQueryable();

            if (predicado is not null)
                query = query.Where(predicado);

            return await query.ToListAsync(cancellationToken);
        }
        public async Task<TEntidad> AgregarAsync<TEntidad>(TEntidad entidad) where TEntidad : class, IEntity, IAggregateRoot
        {
            var entityEntry = await dbContext.Set<TEntidad>().AddAsync(entidad);
            return entityEntry.Entity;
        }


        public Task EjecutaStoredProcedure(string nombre, params object[] parametros)
        {
            throw new NotImplementedException();
        }
        public Task<TResult> EjecutaStoredProcedure<TResult>(string nombre, params object[] parametros)
        {
            throw new NotImplementedException();
        }
        Task IUoWCommand.AgregarVariosAsync<TEntidad>(IEnumerable<TEntidad> entidades)
        {
            throw new NotImplementedException();
        }
        IQueryable<TEntidad> IUoWCommand.ConsultaSql<TEntidad>(string consulta, params object[] parametros)
        {
            throw new NotImplementedException();
        }
        Task IUoWCommand.EliminarAsync<TEntidad>(TEntidad entidad)
        {
            throw new NotImplementedException();
        }
        Task<bool> IUoWCommand.ExisteAsync<TEntidad>(Expression<Func<TEntidad, bool>> predicado, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        TEntidad IUoWCommand.ObtenerDatosOriginales<TEntidad>(TEntidad entidad)
        {
            throw new NotImplementedException();
        }
        Task<TEntidad> IUoWCommand.ObtenerPorIdAsync<TEntidad>(params object[] ids)
        {
            throw new NotImplementedException();
        }
        IQueryable<TResult> IUoWCommand.Proyectar<TEntidad, TResult>(Expression<Func<TEntidad, TResult>> selector, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}