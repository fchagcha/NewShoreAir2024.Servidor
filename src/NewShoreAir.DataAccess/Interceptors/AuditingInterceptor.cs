namespace NewShoreAir.DataAccess.Interceptors
{
    public sealed class AuditingInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
                ModificaFechasAuditoria(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ModificaFechasAuditoria(DbContext context)
        {
            var nombreUsuario = "Admin";

            foreach (var entry in context.ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.FechaCreacion = DateTime.Now;
                    entry.Entity.CreadoPor = nombreUsuario;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModificadoPor = nombreUsuario;
                    entry.Entity.FechaUltimaModificacion = DateTime.Now;
                }
            }
        }
    }
}
