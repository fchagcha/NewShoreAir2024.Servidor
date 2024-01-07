namespace NewShoreAir.Shared.Models
{
    public class VueloApiResponse
    {
        public string departureStation { get; set; }
        public string arrivalStation { get; set; }
        public string flightCarrier { get; set; }
        public string flightNumber { get; set; }
        public int price { get; set; }
    }
}
