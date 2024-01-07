namespace NewShoreAir.Business.Domain.Entidades
{
    public partial class ViajeVuelo
    {
        private Viaje _viaje;
        [ForeignKey(nameof(IdViaje))]
        public Viaje Viaje
        {
            get
            {
                LazyLoader.Load(this, ref _viaje);
                return _viaje;
            }
            set
            {
                _viaje = value;
                IdViaje = value?.Id ?? string.Empty;
            }
        }

        private Vuelo _vuelo;
        [ForeignKey(nameof(IdVuelo))]
        public Vuelo Vuelo
        {
            get
            {
                LazyLoader.Load(this, ref _vuelo);
                return _vuelo;
            }
            set
            {
                _vuelo = value;
                IdVuelo = value?.Id ?? string.Empty;
            }
        }
    }
}