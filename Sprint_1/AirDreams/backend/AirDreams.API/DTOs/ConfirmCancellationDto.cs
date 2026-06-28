using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.DTOs
{
    public class ConfirmCancellationDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}