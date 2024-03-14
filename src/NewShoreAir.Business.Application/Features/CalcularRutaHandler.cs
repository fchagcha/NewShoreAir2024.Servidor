using NewShoreAir.Business.Application.Extensions;

namespace NewShoreAir.Business.Application.Features
{
    public class CalcularRutaHandler(ILogger<CalcularRutaHandler> logger, IProvider provider) : IRequestHandler<CalcularRutaRequest, ViajeResponse>
    {
        private readonly IUoWCommand _unitOfWork = provider.ObtenerUnitOfWork<IUoWCommand>();
        private readonly IVueloApi _vueloApi = provider.ObtenerServicio<IVueloApi>();

        public async Task<ViajeResponse> Handle(CalcularRutaRequest request, CancellationToken cancellationToken)
            => await 
                ObtenerViajeAsync(request)
                .PipeNullCheckAsync(
                    viaje => DevuelveViajeAsync(viaje),
                    viaje => AgregaViajeAsync(viaje, request));

        private async Task<Viaje> ObtenerViajeAsync(CalcularRutaRequest request) 
            => await 
                _unitOfWork
                .ObtenerAsync<Viaje>(x => x.Origen.Equals(request.Origen) && 
                                          x.Destino.Equals(request.Destino));
        private async Task<ViajeResponse> DevuelveViajeAsync(Viaje viaje)
        {
            logger.LogInformation("Se consume Base de datos");
            await Task.Yield();
            return viaje.ToViajeResponse();
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

            var response = new ViajeResponse
            {
                Origen = request.Origen,
                Destino = request.Destino,
                NumeroDeVuelos = vuelosApi.Count(),
                Precio = vuelosApi.Sum(x => x.price),
                Vuelos = vuelosApi.Select(x => x.ToVueloResponseFromApi()).ToList()
            };

            return response;
        }
    }
}