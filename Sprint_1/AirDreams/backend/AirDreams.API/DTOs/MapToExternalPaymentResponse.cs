namespace AirDreams.API.DTOs
{
    public class ExternalPaymentResponseDto
    {
        public string reservationNumber { get; set; } = "";
        public bool firstClass { get; set; }
        public ExternalFlightResponseDto flight { get; set; } = new();
        public ExternalBreakupDto breakup { get; set; } = new();
        public List<ExternalPassengerDto> passengers { get; set; } = new();
        public ExternalBuyerResponseDto buyer { get; set; } = new();
    }

    public class ExternalFlightResponseDto
    {
        public string flightGUID { get; set; } = "";
        public string departureTime { get; set; } = "";
        public string arrivalTime { get; set; } = "";
        public string duration { get; set; } = "";
        public ExternalAirportDto departureAirport { get; set; } = new();
        public ExternalAirportDto arrivalAirport { get; set; } = new();
        public decimal touristPrice { get; set; }
        public decimal firstClassPrice { get; set; }
        public decimal carryOnPrice { get; set; }
        public decimal checkedPrice { get; set; }
    }

    public class ExternalAirportDto
    {
        public string code { get; set; } = "";
        public string name { get; set; } = "";
        public string city { get; set; } = "";
    }

    public class ExternalBreakupDto
    {
        public decimal luggage { get; set; }
        public decimal tickets { get; set; }
        public decimal taxes { get; set; }
        public decimal total { get; set; }
    }

    public class ExternalBuyerResponseDto
    {
        public string firstName { get; set; } = "";
        public string lastName { get; set; } = "";
        public string? lastName2 { get; set; }
        public string phoneNumber { get; set; } = "";
        public string email { get; set; } = "";
    }
}