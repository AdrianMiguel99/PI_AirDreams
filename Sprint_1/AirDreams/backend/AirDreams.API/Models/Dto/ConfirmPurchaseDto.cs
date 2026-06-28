using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.Models.Dtos
{
    public class ConfirmPurchaseDto
    {
        [Required]
        public string TransactionId { get; set; }

        [Required]
        public string BuyerName { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public string? CardNumber { get; set; }
        public string? CardExpiry { get; set; }
        public string? CardCvv { get; set; }

        [Required]
        public List<FlightSegmentDto> Segments { get; set; }

        [Required]
        public string SeatClass { get; set; }
        public decimal PricePerPassenger { get; set; }
        public int PassengerCount { get; set; }

        [Required]
        public List<PassengerDto> Passengers { get; set; }

        public List<LuggagePerPassengerDto>? Luggage { get; set; }
    }

    public class PassengerDto
    {
        public string NamePassenger { get; set; }
        public string LastnamesPassenger { get; set; }
        public DateTime BirthDate { get; set; }
        public string? EmailPassenger { get; set; }
        public string? Telephone { get; set; }
        public string Country { get; set; }    
    }

    public class FlightSegmentDto
    {
        public string FlightNumber { get; set; } = string.Empty;
        public int? RouteId { get; set; }

        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }

        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;

        public AirportInfoDto DepartureAirport { get; set; } = new();
        public AirportInfoDto ArrivalAirport { get; set; } = new();

        public decimal CheckedPrice { get; set; }
        public decimal CarryOnPrice { get; set; }
        public decimal Multiplier { get; set; }
    }

    public class AirportInfoDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class LuggagePerPassengerDto
    {
        public int PassengerIndex { get; set; }
        public List<LuggageItemDto> LuggageItems { get; set; }
    }

    public class LuggageItemDto
    {
        public string Type { get; set; }        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
