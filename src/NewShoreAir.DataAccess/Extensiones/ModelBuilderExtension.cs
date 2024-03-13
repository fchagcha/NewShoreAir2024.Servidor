namespace NewShoreAir.DataAccess.Extensiones
{
    public static partial class ModelBuilderExtension
    {
        public static ModelBuilder RegitrarEntidades(this ModelBuilder modelBuilder)
        {
            var types =
                AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetExportedTypes())
                .Where(x => x.IsClass &&
                           !x.IsAbstract &&
                            x.IsPublic &&
                            Attribute.IsDefined(x, typeof(EntityAttribute)));

            foreach (var type in types)
                modelBuilder.Entity(type);

            return modelBuilder;
        }
        public static ModelBuilder RegitrarIndices(this ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes();

            var propiedadesIndice =
                entityTypes
                .SelectMany(x => x.ClrType
                    .GetProperties()
                    .Where(y => y.GetCustomAttributes<IndexUniqueAttribute>().Any())
                    .Select(y => new { EntityType = x, Property = y }));

            foreach (var item in propiedadesIndice)
                modelBuilder.Entity(item.EntityType.Name).HasIndex(item.Property.Name).IsUnique();

            return modelBuilder;
        }
    }
}