namespace AirDreams.API.DTOs
{
    public class FlightsReportRowDto
    {
        public DateTime FlightDate { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string NumberFlight { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public int? FirstClassPassengers { get; set; }
        public int? TouristPassengers { get; set; }
        public decimal? PassengerRevenue { get; set; }
        public decimal? LuggageRevenue { get; set; }
        public decimal? TotalRevenue { get; set; }
    }

    public class FlightsReportFiltersDto
    {
        public IEnumerable<string> Origins { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> Destinations { get; set; } = Enumerable.Empty<string>();
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
    }
}