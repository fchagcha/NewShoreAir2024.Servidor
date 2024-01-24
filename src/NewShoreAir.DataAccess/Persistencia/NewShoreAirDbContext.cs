

using Fabrela.Domain.Core.Attributes;
using Fabrela.Infraestructura.Data.Extensiones;

namespace NewShoreAir.DataAccess.Persistencia
{
    public class NewShoreAirDbContext : DbContext
    {
        public NewShoreAirDbContext(DbContextOptions<NewShoreAirDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RegitrarEntidades();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var nombreUsuario = "Admin";

            foreach (var entry in ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.FechaCreacion = DateTime.Now;
                    entry.Entity.CreadoPor = nombreUsuario;
                    entry.Entity.ModificadoPor = nombreUsuario;
                    entry.Entity.FechaUltimaModificacion = DateTime.Now;
                    entry.Entity.Activo = true;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModificadoPor = nombreUsuario;
                    entry.Entity.FechaUltimaModificacion = DateTime.Now;
                }
            }

            var response = await base.SaveChangesAsync(cancellationToken);

            return response;
        }

        //public DbSet<Transporte> Transportes { get; set; }
        //public DbSet<Viaje> Viajes { get; set; }
        //public DbSet<ViajeVuelo> ViajeVuelos { get; set; }
        //public DbSet<Vuelo> Vuelos { get; set; }
    }
}