using NewShoreAir.Shared.Models;

namespace NewShoreAir.Business.Application.Extensions
{
    public static class MappingExtensions
    {
        public static VueloResponse ToVueloResponseFromApi(this VueloApiResponse vueloApi)
        {
            if (vueloApi is null)
                return default;

            return new VueloResponse
            {
                Origen = vueloApi.departureStation,
                Destino = vueloApi.arrivalStation,
                Precio = vueloApi.price,
                Transporte = new TransporteResponse
                {
                    Transportista = vueloApi.flightCarrier,
                    Numero = vueloApi.flightNumber
                }
            };
        }

        public static ViajeResponse ToViajeResponse(this Viaje viaje)
        {
            if (viaje is null)
                return default;

            return new ViajeResponse
            {
                Origen = viaje.Origen,
                Destino = viaje.Destino,
                Precio = viaje.Precio,
                NumeroDeVuelos = viaje.NumeroDeVuelos,
                Vuelos = viaje.ViajeVuelos
                        .OrderBy(x =>x.Orden)
                        .Select(x => x.Vuelo.ToVueloResponse())
                        .ToList()
            };
        }
        public static VueloResponse ToVueloResponse(this Vuelo vuelo)
        {
            if (vuelo is null)
                return default;

            return new VueloResponse
            {
                Origen = vuelo.Origen,
                Destino = vuelo.Destino,
                Precio = vuelo.Precio,
                Transporte = vuelo.Transporte.ToTransporteResponse()
            };
        }
        public static TransporteResponse ToTransporteResponse(this Transporte transporte)
        {
            if (transporte is null)
                return default;

            return new TransporteResponse
            {
                Transportista = transporte.Transportista,
                Numero = transporte.Numero
            };
        }

        public static Viaje ToViaje(this ViajeResponse viajeResponse)
        {
            if (viajeResponse is null)
                return default;

            return new Viaje(
                viajeResponse.Origen,
                viajeResponse.Destino,
                viajeResponse.Precio,
                viajeResponse.NumeroDeVuelos);
        }
        public static Vuelo ToVuelo(this VueloResponse vueloResponse, Transporte transporte)
        {
            if (vueloResponse is null)
                return default;

            return new Vuelo(
                transporte,
                vueloResponse.Origen,
                vueloResponse.Destino,
                vueloResponse.Precio);
        }
        public static Transporte ToTransporte(this TransporteResponse transporteResponse)
        {
            if (transporteResponse is null)
                return default;

            return new Transporte(
                transporteResponse.Transportista,
                transporteResponse.Numero);
        }
    }
}
