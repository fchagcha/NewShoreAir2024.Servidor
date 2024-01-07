using NewShoreAir.Shared.Models;

namespace NewShoreAir.Shared.Services.Interfaces
{
    public interface IVueloApi
    {
        Task<List<VueloApiResponse>> ListarVuelosApi();
    }
}