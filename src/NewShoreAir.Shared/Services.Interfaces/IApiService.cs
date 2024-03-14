namespace NewShoreAir.Shared.Services.Interfaces
{
    public interface IApiService
    {
        Task<List<T>> GetFromApiAsync<T>(string uri, string key, bool usaCache = false, int minutosCache = 0);
    }
}