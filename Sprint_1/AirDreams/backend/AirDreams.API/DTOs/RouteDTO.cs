using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirDreams.API.DTOs
{
    public class RouteDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DepartureTime { get; set; } = string.Empty;
        [Required]
        public string ArrivalTime { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public AirportDTO DepartureAirport { get; set; } = new AirportDTO();
        public AirportDTO ArrivalAirport { get; set; } = new AirportDTO();
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }

        public decimal  luggagePrice {get; set; }

        public decimal luggageMaxWeight {get; set; }

        public decimal carryOnPrice {get; set; }
        
        public decimal carryOnMaxWeight {get; set; }

        public decimal porcentageMultiplier {get; set; }
    }
}