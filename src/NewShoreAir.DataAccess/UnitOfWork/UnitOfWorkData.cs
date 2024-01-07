namespace NewShoreAir.DataAccess.UnitOfWork
{
    public class UnitOfWorkData(NewShoreAirDbContext dbContext) : IUnitOfWorkData
    {
        private volatile bool _disposedValue;

        public CancellationToken CancellationToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
            bool ignoreQueryFilter = false) where TEntidad : class, IEntity
        {
            var entidad = dbContext.Set<TEntidad>().Local.AsQueryable().FirstOrDefault(predicado);

            if (entidad is null)
            {
                var query = dbContext.Set<TEntidad>().AsQueryable();

                if (ignoreQueryFilter)
                    query = query.IgnoreQueryFilters();

                entidad = await query.FirstOrDefaultAsync(predicado);
            }

            return entidad;
        }
        public IQueryable<TEntidad> Filtrar<TEntidad>(
            Expression<Func<TEntidad, bool>> predicado = null,
            bool ignoreQueryFilter = false) where TEntidad : class, IEntity
        {
            var query = dbContext.Set<TEntidad>().AsQueryable();

            if (predicado is not null)
                query = query.Where(predicado);

            if (ignoreQueryFilter)
                query = query.IgnoreQueryFilters();

            return query;
        }
        public async Task<List<TEntidad>> ListarAsync<TEntidad>(
            Expression<Func<TEntidad, bool>> predicado = null,
            bool ignoreQueryFilter = false) where TEntidad : class, IEntity
        {
            var query = dbContext.Set<TEntidad>().AsQueryable();

            if (predicado is not null)
                query = query.Where(predicado);

            if (ignoreQueryFilter)
                query = query.IgnoreQueryFilters();

            return await query.ToListAsync();
        }
        public async Task<TEntidad> AgregarAsync<TEntidad>(TEntidad entidad) where TEntidad : class, IEntity, IAggregateRoot
        {
            var entityEntry = await dbContext.Set<TEntidad>().AddAsync(entidad);
            return entityEntry.Entity;
        }

        IQueryable<TResult> IUnitOfWorkData.Proyectar<TEntidad, TResult>(Expression<Func<TEntidad, TResult>> selector, bool ignoreQueryFilter)
        {
            throw new NotImplementedException();
        }

        IQueryable<TEntidad> IUnitOfWorkData.ConsultaSql<TEntidad>(string consulta, params object[] parametros)
        {
            throw new NotImplementedException();
        }

        Task<TEntidad> IUnitOfWorkData.ObtenerPorIdAsync<TEntidad>(params object[] ids)
        {
            throw new NotImplementedException();
        }

        TEntidad IUnitOfWorkData.ObtenerDatosOriginales<TEntidad>(TEntidad entidad)
        {
            throw new NotImplementedException();
        }

        public Task EjecutaStoredProcedure(string nombre, params object[] parametros)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> EjecutaStoredProcedure<TResult>(string nombre, params object[] parametros)
        {
            throw new NotImplementedException();
        }


        Task IUnitOfWorkData.EliminarAsync<TEntidad>(TEntidad entidad)
        {
            throw new NotImplementedException();
        }

        Task IUnitOfWorkData.AgregarVariosAsync<TEntidad>(IEnumerable<TEntidad> entidades)
        {
            throw new NotImplementedException();
        }
    }
}