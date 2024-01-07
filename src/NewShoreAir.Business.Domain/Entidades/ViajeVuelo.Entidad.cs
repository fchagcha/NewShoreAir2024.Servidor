namespace NewShoreAir.Business.Domain.Entidades
{
    public partial class ViajeVuelo : BaseEntity<string>
    {
        private ILazyLoader LazyLoader { get; set; }

        public ViajeVuelo(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        public ViajeVuelo(Viaje viaje, Vuelo vuelo, int orden)
        {
            Id = Guid.NewGuid().ToString();
            Viaje = viaje;
            Vuelo = vuelo;
            Orden = orden;
        }

        [Required]
        public string IdViaje { get; private set; }

        [Required]
        public string IdVuelo { get; private set; }

        [Required]
        public int Orden { get; private set; }


    }
}