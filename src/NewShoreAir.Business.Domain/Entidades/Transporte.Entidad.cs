namespace NewShoreAir.Business.Domain.Entidades
{
    [Table("Transportes")]
    public partial class Transporte : BaseEntity<string>
    {
        private ILazyLoader LazyLoader { get; set; }

        public Transporte(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        public Transporte(string transportista, string numero)
        {
            Id = Guid.NewGuid().ToString();
            Transportista = transportista;
            Numero = numero;
        }

        [Required]
        [StringLength(50)]
        public string Transportista { get; private set; }

        [Required]
        [StringLength(50)]
        public string Numero { get; private set; }
    }
}