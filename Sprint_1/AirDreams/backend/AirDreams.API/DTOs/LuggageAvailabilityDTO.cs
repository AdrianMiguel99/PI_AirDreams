using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.DTOs
{
    public class LuggageAvailabilityDTO
    {
        [Required(ErrorMessage = "El ID del vuelo es obligatorio")]
        public string flightId { get; set; }

        [Required(ErrorMessage = "El peso del equipaje es obligatorio")]
        [Range(0.00, double.MaxValue, ErrorMessage = "El peso del equipaje debe ser un número positivo")]
        public decimal LuggageWeight { get; set; }

        [Required(ErrorMessage = "El peso de la maleta de mano es obligatorio")]
        [Range(0.00, double.MaxValue, ErrorMessage = "El peso de la maleta de mano debe ser un número positivo")]
        public decimal CarryOnWeight { get; set; }
    }
}
