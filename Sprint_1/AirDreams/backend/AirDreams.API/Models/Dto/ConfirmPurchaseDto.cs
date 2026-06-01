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
        public string EmailPassenger { get; set; }
        public string? Telephone { get; set; }
        public string Country { get; set; }    
    }

    public class FlightSegmentDto
    {
        public string FlightNumber { get; set; }
        public int? RouteId { get; set; }
        public decimal CheckedPrice { get; set; }
        public decimal CarryOnPrice { get; set; }
        public decimal Multiplier { get; set; }
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