using System.ComponentModel.DataAnnotations;
namespace AirDreams.API.DTOs
{
    public class CreatePassengerDto
    {
        [Required]
        [Range(1, 99999)]
        public int IdPassenger { get; set; }

        [Required]
        [StringLength(30)]
        public string NamePassenger { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string LastnamesPassenger { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string EmailPassenger { get; set; } = string.Empty;

        public long? Telephone { get; set; }

        [Required]
        [StringLength(50)]
        public string Country { get; set; } = string.Empty;
    }
}
