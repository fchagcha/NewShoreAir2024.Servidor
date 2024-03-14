using NewShoreAir.Business.Application.Extensions;

namespace NewShoreAir.Business.Application.Features
{
    public class CalcularRutaHandler(ILogger<CalcularRutaHandler> logger, IProvider provider) : IRequestHandler<CalcularRutaRequest, ViajeResponse>
    {
        private readonly IUoWCommand _unitOfWork = provider.ObtenerUnitOfWork<IUoWCommand>();
        private readonly IVueloApi _vueloApi = provider.ObtenerServicio<IVueloApi>();

        public async Task<ViajeResponse> Handle(CalcularRutaRequest request, CancellationToken cancellationToken)
            => await
                _unitOfWork
                .ObtenerAsync<Viaje>(
                    x => x.Origen.Equals(request.Origen) &&
                         x.Destino.Equals(request.Destino))
                .PipeNullCheckAsync(
                    viaje => DevuelveViajeAsync(viaje),
                    viaje => AgregaViajeAsync(viaje, request));

        private async Task<ViajeResponse> DevuelveViajeAsync(Viaje viaje)
        {
            logger.LogInformation("Se consume Base de datos");

            var viajeVuelos = await
                _unitOfWork
                .Filtrar<ViajeVuelo>(x => x.IdViaje == viaje.Id)
                .OrderBy(x => x.Orden)
                .Select(x => new
                {
                    x.Viaje.Origen,
                    x.Viaje.Destino,
                    x.Viaje.Precio,
                    x.Viaje.NumeroDeVuelos,
                    VueloOrigen = x.Vuelo.Origen,
                    VueloDestino = x.Vuelo.Destino,
                    VueloPrecio = x.Vuelo.Precio,
                    VueloTransporteTransportista = x.Vuelo.Transporte.Transportista,
                    VueloTransporteNumero = x.Vuelo.Transporte.Numero
                })
                .ToListAsync();

            var response =
                viajeVuelos
                .GroupBy(x => new
                {
                    x.Origen,
                    x.Destino,
                    x.Precio,
                    x.NumeroDeVuelos
                })
                .Select(x => new ViajeResponse
                {
                    Origen = x.Key.Origen,
                    Destino = x.Key.Destino,
                    Precio = x.Key.Precio,
                    NumeroDeVuelos = x.Key.NumeroDeVuelos,
                    Vuelos = x.Select(x => new VueloResponse
                    {
                        Origen = x.VueloOrigen,
                        Destino = x.VueloDestino,
                        Precio = x.VueloPrecio,
                        Transporte = new TransporteResponse
                        {
                            Transportista = x.VueloTransporteTransportista,
                            Numero = x.VueloTransporteNumero
                        }
                    })
                    .ToList()
                })
                .First();

            return response;
        }
        private async Task<ViajeResponse> AgregaViajeAsync(Viaje viaje, CalcularRutaRequest request)
        {
            logger.LogInformation("Se consume API de NEWSHORE AIR");

            var response = await RetornaVuelosDesdeApiAsync(request);

            var transportes = await
                _unitOfWork
                .ListarAsync<Transporte>();

            var vuelos = await
                _unitOfWork
                .ListarAsync<Vuelo>();

            viaje = response
                .ToViaje()
                .IniciaViaje();

            response
            .Vuelos
            .ForEach(vueloApi =>
            {
                var trasnporteApi = vueloApi.Transporte;

                var trasnporte =
                    transportes.FirstOrDefault(
                        x => x.Transportista.Equals(trasnporteApi.Transportista) &&
                             x.Numero.Equals(trasnporteApi.Numero));

                trasnporte ??= trasnporteApi.ToTransporte();

                var vuelo =
                    vuelos.FirstOrDefault(
                        x => x.Origen.Equals(vueloApi.Origen) &&
                             x.Destino.Equals(vueloApi.Destino));

                vuelo ??= vueloApi.ToVuelo(trasnporte);

                viaje.AgregaVueloAViaje(vuelo);
            });

            viaje.EstableceCostoDeViaje();

            await _unitOfWork.AgregarAsync(viaje);

            return response;
        }
        private async Task<ViajeResponse> RetornaVuelosDesdeApiAsync(CalcularRutaRequest request)
        {
            var vuelosApi = await _vueloApi.ObtenerRutaDeVuelos(
                request.Origen,
                request.Destino,
                request.NumeroMaximoDeVuelos);

            var response = new ViajeResponse();
            response.Origen = request.Origen;
            response.Destino = request.Destino;
            response.NumeroDeVuelos = vuelosApi.Count();
            response.Precio = vuelosApi.Sum(x => x.price);
            response.Vuelos =
                vuelosApi
                .Select(x =>
                {
                    var transporteDto = new TransporteResponse()
                    {
                        Numero = x.flightNumber,
                        Transportista = x.flightCarrier
                    };

                    var vueloDto = new VueloResponse()
                    {
                        Origen = x.departureStation,
                        Destino = x.arrivalStation,
                        Precio = x.price,
                        Transporte = transporteDto
                    };

                    return vueloDto;
                })
                .ToList();

            return response;
        }
    }
}