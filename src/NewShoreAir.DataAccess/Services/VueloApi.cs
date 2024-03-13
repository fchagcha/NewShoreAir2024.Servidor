namespace NewShoreAir.DataAccess.Services
{
    public class VueloApi : IVueloApi
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly VuelosApiSettings _vuelosApiSettings;

        public VueloApi(IProvider provider)
        {
            _cache = provider.ObtenerServicio<IMemoryCache>();
            _vuelosApiSettings = provider.ObtenerServicio<IOptions<VuelosApiSettings>>().Value;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { return true; }
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_vuelosApiSettings.Uri)
            };
        }

        public async Task<List<VueloApiResponse>> ListarVuelosApi()
        {
            var key = _vuelosApiSettings.Key;
            var minutosCache = _vuelosApiSettings.MinutosCache;

            var vuelos = await _cache.GetOrCreateAsync(key, async entry =>
            {
                try
                {
                    var response = await _httpClient.GetAsync(key);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var vuelos = JsonConvert.DeserializeObject<List<VueloApiResponse>>(json);

                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutosCache);

                    return vuelos;
                }
                catch (HttpRequestException ex)
                {
                    throw new CustomException("Error al obtener vuelos de la API." + ex.Message);
                }
                catch (JsonException ex)
                {
                    throw new CustomException("Error al deserializar la respuesta JSON de la API." + ex.Message);
                }
            });

            return vuelos;
        }
    }
}