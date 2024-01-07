namespace NewShoreAir.Business.Domain.Entidades
{
    public partial class Vuelo
    {
        private HashSet<ViajeVuelo> _viajeVuelos;
        [InverseProperty(nameof(ViajeVuelo.Vuelo))]
        public IEnumerable<ViajeVuelo> ViajeVuelos
        {
            get { LazyLoader.Load(this, ref _viajeVuelos); return _viajeVuelos; }
        }
    }
}