namespace NewShoreAir.DataAccess.Services
{
    public class ApiService(IMemoryCache cache) : IApiService
    {
        private readonly IMemoryCache _cache = cache;

        public async Task<List<T>> GetFromApiAsync<T>(string uri, string key, bool usaCache = false, int minutosCache = 0)
        {
            if (usaCache)
            {
                var datos = await _cache.GetOrCreateAsync(key, async entry =>
                {
                    var datos = await GetFromApiWithoutCacheAsync<T>(uri, key);

                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutosCache);

                    return datos;
                });

                return datos;
            }
            else
            {
                return await GetFromApiWithoutCacheAsync<T>(uri, key);
            }
        }

        private async Task<List<T>> GetFromApiWithoutCacheAsync<T>(string uri, string key)
        {
            try
            {
                var response = await CreaHttpClient(uri).GetAsync(key);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var datos = JsonConvert.DeserializeObject<List<T>>(json);

                return datos;
            }
            catch (HttpRequestException ex)
            {
                throw new CustomException("Error al obtener datos de la API." + ex.Message);
            }
            catch (JsonException ex)
            {
                throw new CustomException("Error al deserializar la respuesta JSON de la API." + ex.Message);
            }
        }
        private static HttpClient CreaHttpClient(string uri)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { return true; }
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(uri)
            };
        }
    }
}