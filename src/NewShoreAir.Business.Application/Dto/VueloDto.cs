namespace NewShoreAir.Business.Application.Dto
{
    public class VueloDto
    {
        public string Origen { get; set; }
        public string Destino { get; set; }
        public decimal Precio { get; set; }
        public TransporteDto Transporte { get; set; }
    }
}
