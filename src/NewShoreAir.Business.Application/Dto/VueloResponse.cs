namespace NewShoreAir.Business.Application.Dto
{
    public class VueloResponse
    {
        public string Origen { get; set; }
        public string Destino { get; set; }
        public decimal Precio { get; set; }
        public TransporteResponse Transporte { get; set; }
    }
}
