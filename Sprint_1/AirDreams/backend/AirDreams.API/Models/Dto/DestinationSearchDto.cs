using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.Models.Dto
{
    public class DestinationSearchDto
    {
        [Required(ErrorMessage = "Destino es requerido")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El código del destino debe tener exactamente 3 caracteres")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Fecha de salida es requerida")]
        public DateTime DepartureDate { get; set; }

        [Required(ErrorMessage = "Fecha de retorno es requerida")]
        public DateTime ReturnDate { get; set; }

        [Required(ErrorMessage = "Número de pasajeros es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de pasajeros debe ser al menos 1")]
        public int Passengers { get; set; }

        public void Validate()
        {
            if (DepartureDate > ReturnDate)
                throw new ArgumentException("INVALID_DATE_RANGE: El rango de fechas es inválido.");
        }
    }
}
