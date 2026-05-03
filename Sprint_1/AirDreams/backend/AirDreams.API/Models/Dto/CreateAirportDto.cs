using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.Models.Dtos
{
    public class CreateAirportDto
    {
        [Required(ErrorMessage = "El código es obligatorio.")]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Deben ser 3 letras mayúsculas, sin números.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(200, ErrorMessage = "Máximo 200 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(56)]
        public string Country { get; set; } = string.Empty;

        public TimeSpan? TimeZone { get; set; }
    }
}