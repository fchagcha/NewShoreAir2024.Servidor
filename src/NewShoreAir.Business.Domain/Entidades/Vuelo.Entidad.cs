namespace NewShoreAir.Business.Domain.Entidades
{
    public partial class Vuelo : BaseEntity<string>
    {
        private ILazyLoader LazyLoader { get; set; }

        public Vuelo(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        public Vuelo(Transporte transporte,
            string origen,
            string destino,
            decimal precio)
        {
            Id = Guid.NewGuid().ToString();
            Transporte = transporte;
            Origen = origen;
            Destino = destino;
            Precio = precio;
        }

        [Required]
        public string IdTransporte { get; private set; }

        [Required]
        [StringLength(5)]
        public string Origen { get; private set; }

        [Required]
        [StringLength(5)]
        public string Destino { get; private set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; private set; }

        private Transporte _transporte;
        [ForeignKey(nameof(IdTransporte))]
        public Transporte Transporte
        {
            get
            {
                LazyLoader.Load(this, ref _transporte);
                return _transporte;
            }
            set
            {
                _transporte = value;
                IdTransporte = value?.Id ?? string.Empty;
            }
        }
    }
}