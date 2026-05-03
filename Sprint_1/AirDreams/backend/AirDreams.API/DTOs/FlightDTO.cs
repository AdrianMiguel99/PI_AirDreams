namespace AirDreams.API.DTOs
{
    public class FlightDTO
    {
        public string FlightGUID { get; set; }
        public decimal CarryOnPrice { get; set; }
        public decimal CheckedPrice { get; set; }
        public RouteDTO Route { get; set; }
    }
}