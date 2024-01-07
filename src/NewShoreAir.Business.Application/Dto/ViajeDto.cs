namespace NewShoreAir.Business.Application.Dto
{
    public class ViajeDto
    {
        public string Origen { get; set; }
        public string Destino { get; set; }
        public decimal Precio { get; set; }
        public int NumeroDeVuelos { get; set; }
        public List<VueloDto> Vuelos { get; set; }
    }
}
