using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.DTOs
{
    public class UpdateAirportDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres.")]
        public string Name { get; set; } = string.Empty;
    }
}