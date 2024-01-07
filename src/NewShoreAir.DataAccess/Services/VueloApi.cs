

namespace NewShoreAir.DataAccess.Services
{
    public class VueloApi : IVueloApi
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly VuelosApiSettings _vuelosApiSettings;

        public VueloApi(IMemoryCache cache, IHttpClientFactory httpClientFactory, IOptions<VuelosApiSettings> vuelosApiSettings)
        {
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient();

            _vuelosApiSettings = vuelosApiSettings.Value;

            string uri = vuelosApiSettings.Value.Uri;
            _httpClient.BaseAddress = new Uri(uri);
        }

        public async Task<List<VueloApiResponse>> ListarVuelosApi()
        {
            string key = _vuelosApiSettings.Key;
            int minutosCache = _vuelosApiSettings.MinutosCache;

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