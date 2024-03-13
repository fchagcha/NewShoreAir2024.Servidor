namespace NewShoreAir.DataAccess.Persistencia
{
    public class NewShoreAirDbContext(DbContextOptions<NewShoreAirDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RegitrarEntidades()
                        .RegitrarIndices();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}