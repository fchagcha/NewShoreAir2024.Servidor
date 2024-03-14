namespace NewShoreAir.DataAccess.Services
{
    public class VueloApi(IApiService apiService, IOptions<VuelosApiSettings> vuelosApiSettings) : IVueloApi
    {
        private readonly IApiService _apiService = apiService;
        private readonly VuelosApiSettings _vuelosApiSettings = vuelosApiSettings.Value;

        public async Task<IEnumerable<VueloApiResponse>> ObtenerRutaDeVuelos(string origen, string destino, int numeroDeVuelos)
        {
            var uri = _vuelosApiSettings.Uri;
            var key = _vuelosApiSettings.Key;
            var minutosCache = _vuelosApiSettings.MinutosCache;

            var vuelosApi = await _apiService.GetFromApiAsync<VueloApiResponse>(uri, key, true, minutosCache);

            if (vuelosApi.Count == 0)
                return [];

            var existeOrigen =
                vuelosApi
                .Exists(x => x.departureStation.Equals(origen));

            if (!existeOrigen)
            {
                var mensajeError = $"Origen ingresado {origen}, no registrado en lista de vuelos";
                throw new CustomException(mensajeError);
            }

            var existeDestino =
                vuelosApi
                .Exists(x => x.arrivalStation.Equals(destino));

            if (!existeDestino)
            {
                var mensajeError = $"Destino ingresado {destino}, no registrado en lista de vuelos";
                throw new CustomException(mensajeError);
            }

            var rutaDeViaje = BuscarRutaDeVuelos(origen, destino, vuelosApi);

            if (rutaDeViaje.Any())
            {
                var mensajeError = $"Su consulta no puede ser procesada, para Origen {origen} y Destino {destino}.";
                throw new CustomException(mensajeError);
            }

            return rutaDeViaje;
        }

        private static IEnumerable<VueloApiResponse> BuscarRutaDeVuelos(
           string origen,
           string destino,
           List<VueloApiResponse> vuelos)
        {
            var vueloDirecto =
                vuelos
                .Find(x => x.departureStation.Equals(origen) &&
                           x.arrivalStation.Equals(destino));

            if (vueloDirecto is not null)
                return [vueloDirecto];

            var rutas =
                vuelos
                .Where(v => v.departureStation.Equals(origen))
                .SelectMany(primerVuelo =>
                {
                    var rutaDeVuelos = new HashSet<VueloApiResponse> { primerVuelo };
                    return BuscarRutaDeVuelosRecursivo(primerVuelo, destino, vuelos, rutaDeVuelos);
                });

            return rutas.OrderBy(x => x.Count()).FirstOrDefault() ?? Enumerable.Empty<VueloApiResponse>();
        }
        private static IEnumerable<IEnumerable<VueloApiResponse>> BuscarRutaDeVuelosRecursivo(
            VueloApiResponse vueloActual,
            string destino,
            List<VueloApiResponse> vuelos,
            HashSet<VueloApiResponse> rutaDeVuelos)
        {
            if (vueloActual.arrivalStation.Equals(destino))
            {
                yield return rutaDeVuelos.ToList();
            }
            else
            {
                var destinosSiguientes =
                    vuelos
                    .Where(x => x.departureStation.Equals(vueloActual.arrivalStation) &&
                                !rutaDeVuelos.Contains(x));

                foreach (var siguienteVuelo in destinosSiguientes)
                {
                    rutaDeVuelos.Add(siguienteVuelo);

                    foreach (var ruta in BuscarRutaDeVuelosRecursivo(siguienteVuelo, destino, vuelos, rutaDeVuelos))
                        yield return ruta;

                    rutaDeVuelos.Remove(siguienteVuelo);
                }
            }
        }
    }
}