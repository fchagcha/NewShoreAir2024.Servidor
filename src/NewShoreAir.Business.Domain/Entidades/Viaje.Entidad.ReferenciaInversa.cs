namespace NewShoreAir.Business.Domain.Entidades
{
    public partial class Viaje
    {
        private HashSet<ViajeVuelo> _viajeVuelos;
        [InverseProperty(nameof(ViajeVuelo.Viaje))]
        public IEnumerable<ViajeVuelo> ViajeVuelos
        {
            get { LazyLoader.Load(this, ref _viajeVuelos); return _viajeVuelos; }
        }
    }
}