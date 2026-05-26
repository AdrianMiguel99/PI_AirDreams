namespace AirDreams.API.DTOs
{
public class FlightItineraryDTO
    {
        public string ItineraryId { get; set; }
        public int Stops { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
        public List<FlightSegmentDTO> Segments { get; set; } = new List<FlightSegmentDTO>();
    }

}