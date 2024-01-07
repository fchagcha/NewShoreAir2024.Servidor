
namespace NewShoreAir.Business.Application.Models
{
    public class CalcularRutaResponse : ViajeDto
    {
    }

    public class CalcularRutaRequest : IRequest<CalcularRutaResponse>
    {
        public string Origen { get; set; }
        public string Destino { get; set; }
        public int NumeroMaximoDeVuelos { get; set; }
    }
}
