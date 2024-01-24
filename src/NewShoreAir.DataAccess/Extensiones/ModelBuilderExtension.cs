using Fabrela.Domain.Core.Attributes;

namespace Fabrela.Infraestructura.Data.Extensiones
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
    }
}