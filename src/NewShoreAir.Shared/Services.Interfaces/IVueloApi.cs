using NewShoreAir.Shared.Models;

namespace NewShoreAir.Shared.Services.Interfaces
{
    public interface IVueloApi
    {
        Task<IEnumerable<VueloApiResponse>> ObtenerRutaDeVuelos(
            string origen,
            string destino,
            int numeroDeVuelos);
    }
}