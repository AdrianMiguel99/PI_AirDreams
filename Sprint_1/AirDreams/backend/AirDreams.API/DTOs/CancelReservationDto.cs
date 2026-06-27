using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.DTOs
{
    public class CancelReservationDto
    {
        [Required]
        public string TransactionId { get; set; } = string.Empty;
    }
}