namespace NewShoreAir.Business.Domain.Entidades
{
    public partial class Viaje
    {
        private decimal _precioViaje;
        private int _orden;
        private int _numeroDeVuelos;

        public Viaje IniciaViaje()
        {
            _precioViaje = decimal.Zero;
            _orden = 1;
            _viajeVuelos ??= new();
            _numeroDeVuelos = 0;

            return this;
        }
        public Viaje AgregaVueloAViaje(Vuelo vuelo)
        {
            ViajeVuelo viajeVuelo = new(this, vuelo, _orden);

            _viajeVuelos.Add(viajeVuelo);

            _precioViaje += vuelo.Precio;
            _orden++;
            _numeroDeVuelos++;

            return this;
        }
        public Viaje EstableceCostoDeViaje()
        {
            Precio = _precioViaje;
            NumeroDeVuelos = _numeroDeVuelos;

            return this;
        }
    }
}