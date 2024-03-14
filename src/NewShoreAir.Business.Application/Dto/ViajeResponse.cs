namespace NewShoreAir.Business.Application.Dto
{
    public class ViajeResponse
    {
        public string Origen { get; set; }
        public string Destino { get; set; }
        public decimal Precio { get; set; }
        public int NumeroDeVuelos { get; set; }
        public List<VueloResponse> Vuelos { get; set; }
    }
}
