namespace NewShoreAir.Business.Domain.Entidades
{
    [Table("Viajes")]
    public partial class Viaje : BaseEntity<string>, IAggregateRoot
    {
        private ILazyLoader LazyLoader { get; set; }
        public Viaje(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        public Viaje(
            string origen,
            string destino,
            decimal precio,
            int numeroDeVuelos)
        {
            Id = Guid.NewGuid().ToString();
            Origen = origen;
            Destino = destino;
            Precio = precio;
            NumeroDeVuelos = numeroDeVuelos;
        }

        [Required]
        [StringLength(5)]
        public string Origen { get; private set; }

        [Required]
        [StringLength(5)]
        public string Destino { get; private set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; private set; }

        [Required]
        public int NumeroDeVuelos { get; private set; }
    }
}