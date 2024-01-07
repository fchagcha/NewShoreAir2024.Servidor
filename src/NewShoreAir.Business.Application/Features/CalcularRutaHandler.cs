namespace NewShoreAir.Business.Application.Features
{
    public class CalcularRutaHandler : IRequestHandler<CalcularRutaRequest, CalcularRutaResponse>
    {
        private readonly ILogger<CalcularRutaHandler> _logger;
        private readonly IUnitOfWorkData _unitOfWork;
        private readonly IVueloApi _vueloApi;
        private readonly IMapper _mapper;

        public CalcularRutaHandler(ILogger<CalcularRutaHandler> logger, IUnitOfWorkProvider unitOfWorkProvider)
        {
            _logger = logger;
            _unitOfWork = _unitOfWork = unitOfWorkProvider.ObtenerUnitOfWork<IUnitOfWorkData>();
            _vueloApi = unitOfWorkProvider.ObtenerServicio<IVueloApi>();
            _mapper = unitOfWorkProvider.ObtenerServicio<IMapper>();
        }

        public async Task<CalcularRutaResponse> Handle(CalcularRutaRequest request, CancellationToken cancellationToken)
        {
            var response = new CalcularRutaResponse();

            var viaje = await
                _unitOfWork
                .ObtenerAsync<Viaje>(
                    x => x.Origen.Equals(request.Origen) &&
                         x.Destino.Equals(request.Destino));

            response = await DevuelveViajeAsync(response, viaje, request);

            response = await AgregaViajeAsync(response, viaje, request);

            return response;
        }

        private async Task<CalcularRutaResponse> DevuelveViajeAsync(CalcularRutaResponse response, Viaje viaje, CalcularRutaRequest request)
        {
            if (viaje is null)
                return response;

            if (request.NumeroMaximoDeVuelos > 0 &&
                viaje.NumeroDeVuelos > request.NumeroMaximoDeVuelos)
            {
                throw new CustomException("Lo sentimos, no pudimos encontrar una ruta de viaje que coincida exactamente con la cantidad de vuelos que estás buscando");
            }

            _logger.LogInformation("Se consume Base de datos");

            var viajeVuelos = await
                _unitOfWork
                .Filtrar<ViajeVuelo>(x => x.IdViaje == viaje.Id)
                .Include(x => x.Viaje)
                .Include(x => x.Vuelo)
                    .ThenInclude(x => x.Transporte)
                .OrderBy(x => x.Orden)
                .ToListAsync();

            response = _mapper.Map<CalcularRutaResponse>(viaje);
            response.Vuelos =
                viajeVuelos
                .Select(viajeVuelo =>
                {
                    var vuelo = viajeVuelo.Vuelo;
                    var transporte = vuelo.Transporte;

                    var transporteDto = _mapper.Map<TransporteDto>(transporte);

                    var vueloDto = _mapper.Map<VueloDto>(vuelo);
                    vueloDto.Transporte = transporteDto;

                    return vueloDto;
                })
                .ToList();

            response.NumeroDeVuelos = viaje.NumeroDeVuelos;

            return response;
        }
        private async Task<CalcularRutaResponse> AgregaViajeAsync(CalcularRutaResponse response, Viaje viaje, CalcularRutaRequest request)
        {
            if (viaje is not null)
                return response;

            response = await RetornaVuelosDesdeApiAsync(request);

            var transportes = await
                _unitOfWork
                .ListarAsync<Transporte>();

            var vuelos = await
                _unitOfWork
                .ListarAsync<Vuelo>();

            viaje = _mapper.Map<Viaje>(response);

            viaje.IniciaViaje();

            response
            .Vuelos
            .ForEach(vueloApi =>
            {
                var trasnporteApi = vueloApi.Transporte;

                var trasnporte =
                    transportes.FirstOrDefault(
                        x => x.Transportista.Equals(trasnporteApi.Transportista) &&
                             x.Numero.Equals(trasnporteApi.Numero));

                trasnporte ??= new(trasnporteApi.Transportista, trasnporteApi.Numero);

                var vuelo =
                    vuelos.FirstOrDefault(
                        x => x.Origen.Equals(vueloApi.Origen) &&
                             x.Destino.Equals(vueloApi.Destino));

                vuelo ??= new(trasnporte, vueloApi.Origen, vueloApi.Destino, vueloApi.Precio);

                viaje.AgregaVueloAViaje(vuelo);
            });

            viaje.EstableceCostoDeViaje();

            if (request.NumeroMaximoDeVuelos > 0 &&
                viaje.NumeroDeVuelos > request.NumeroMaximoDeVuelos)
            {
                throw new CustomException("Lo sentimos, no pudimos encontrar una ruta de viaje que coincida exactamente con la cantidad de vuelos que estás buscando");
            }

            await _unitOfWork.AgregarAsync(viaje);

            response.NumeroDeVuelos = viaje.NumeroDeVuelos;

            return response;
        }
        private async Task<CalcularRutaResponse> RetornaVuelosDesdeApiAsync(CalcularRutaRequest request)
        {
            var vuelosApi = await _vueloApi.ListarVuelosApi();

            _logger.LogInformation("Se consume API de NEWSHORE AIR");

            if (!vuelosApi.Any())
            {
                _logger.LogInformation("API NEWSHORE AIR, no cuenta con vuelos ingresados");
                return new();
            }

            var existeOrigen =
                vuelosApi
                .Any(x => x.departureStation.Equals(request.Origen));

            if (!existeOrigen)
            {
                var mensajeError = $"Origen ingresado {request.Origen}, no registrado en lista de vuelos";

                _logger.LogError(mensajeError);
                throw new CustomException(mensajeError);
            }

            var existeDestino =
                vuelosApi
                .Any(x => x.arrivalStation.Equals(request.Destino));

            if (!existeDestino)
            {
                var mensajeError = $"Destino ingresado {request.Destino}, no registrado en lista de vuelos";

                _logger.LogError(mensajeError);
                throw new CustomException(mensajeError);
            }

            var rutaDeViaje = BuscarRutaDeVuelos(request.Origen, request.Destino, vuelosApi);

            if (!rutaDeViaje.Any())
            {
                var mensajeError = $"Su consulta no puede ser procesada, para Origen {request.Origen} y Destino {request.Destino}.";

                _logger.LogError(mensajeError);
                throw new CustomException(mensajeError);
            }

            var response = new CalcularRutaResponse();

            if (rutaDeViaje.Any())
            {
                response.Origen = request.Origen;
                response.Destino = request.Destino;

                response.Vuelos =
                    rutaDeViaje
                    .Select(x =>
                    {
                        var transporteDto = new TransporteDto()
                        {
                            Numero = x.flightNumber,
                            Transportista = x.flightCarrier
                        };

                        var vueloDto = new VueloDto()
                        {
                            Origen = x.departureStation,
                            Destino = x.arrivalStation,
                            Precio = x.price,
                            Transporte = transporteDto
                        };

                        return vueloDto;
                    })
                    .ToList();

                response.Precio =
                    response
                    .Vuelos
                    ?.Sum(x => x.Precio) ?? decimal.Zero;
            }

            return response;
        }
        public static List<VueloApiResponse> BuscarRutaDeVuelos(
            string origen,
            string destino,
            List<VueloApiResponse> vuelos)
        {
            var vueloDirecto =
                vuelos.FirstOrDefault(
                    x => x.departureStation.Equals(origen) &&
                         x.arrivalStation.Equals(destino));

            if (vueloDirecto is not null)
                return new List<VueloApiResponse> { vueloDirecto };

            var rutas = new List<List<VueloApiResponse>>();

            vuelos
                .Where(v => v.departureStation.Equals(origen))
                .ToList()
                .ForEach(primerVuelo =>
                {
                    var rutaDeVuelos = new List<VueloApiResponse> { primerVuelo };

                    BuscarRutaDeVuelosRecursivo(
                        primerVuelo,
                        destino,
                        vuelos,
                        rutaDeVuelos,
                        rutas);
                });

            var ruta =
                rutas
                ?.OrderBy(x => x.Count)
                ?.FirstOrDefault() ?? new List<VueloApiResponse>();

            return ruta;
        }
        private static void BuscarRutaDeVuelosRecursivo(
            VueloApiResponse vueloActual,
            string destino,
            List<VueloApiResponse> vuelos,
            List<VueloApiResponse> rutaDeVuelos,
            List<List<VueloApiResponse>> rutas)
        {
            if (vueloActual.arrivalStation.Equals(destino))
            {
                rutas.Add(new List<VueloApiResponse>(rutaDeVuelos));
                return;
            }

            var destinosSiguientes =
                vuelos
                .Where(x => x.departureStation.Equals(vueloActual.arrivalStation) &&
                            !rutaDeVuelos.Contains(x))
                .ToList();

            if (destinosSiguientes is null || destinosSiguientes.Count == 0)
                return;

            destinosSiguientes?.ForEach(siguienteVuelo =>
            {
                rutaDeVuelos.Add(siguienteVuelo);
                BuscarRutaDeVuelosRecursivo(siguienteVuelo, destino, vuelos, rutaDeVuelos, rutas);
                rutaDeVuelos.Remove(siguienteVuelo);
            });
        }
    }
}