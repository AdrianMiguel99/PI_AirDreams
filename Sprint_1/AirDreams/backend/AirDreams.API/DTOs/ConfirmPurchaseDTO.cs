using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.DTOs
{
    public class ConfirmPurchaseDTO
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
        public List<FlightSegmentDTO> Segments { get; set; }

        [Required]
        public string SeatClass { get; set; }
        public decimal PricePerPassenger { get; set; }
        public int PassengerCount { get; set; }

        [Required]
        public List<PassengerDTO> Passengers { get; set; }

        public List<LuggagePerPassengerDto>? Luggage { get; set; }
    }


}
