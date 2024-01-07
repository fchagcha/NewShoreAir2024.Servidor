namespace NewShoreAir.Business.Domain.Entidades
{
    public partial class Transporte
    {
        private HashSet<Vuelo> _vuelos;
        [InverseProperty(nameof(Vuelo.Transporte))]
        public IEnumerable<Vuelo> Vuelos
        {
            get { LazyLoader.Load(this, ref _vuelos); return _vuelos; }
        }
    }
}