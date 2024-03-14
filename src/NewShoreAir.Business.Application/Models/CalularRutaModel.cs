
namespace NewShoreAir.Business.Application.Models
{
    public record CalcularRutaRequest(
        string Origen,
        string Destino,
        int NumeroMaximoDeVuelos) : IRequest<ViajeResponse>;
}