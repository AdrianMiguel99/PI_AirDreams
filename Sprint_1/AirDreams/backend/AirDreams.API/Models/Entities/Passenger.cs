using System.ComponentModel.DataAnnotations;
namespace AirDreams.API.Models.Entities

{
    public class Passenger
    {
        [Key]
        [Required(ErrorMessage = "La identificacion del pasajero es requerida.")]
        [Range(1, 99999, ErrorMessage = "La identificacion del pasajero debe estar entre 1 y 99999.")]
        public int IdPassenger { get; set; }

        [Required(ErrorMessage = "El nombre del pasajero es requerido.")]
        [StringLength(30, ErrorMessage = "El nombre del pasajero no puede superar los 30 caracteres.")]
        public string NamePassenger { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos del pasajero son requeridos.")]
        [StringLength(30, ErrorMessage = "Los apellidos del pasajero no pueden superar los 30 caracteres.")]
        public string LastnamesPassenger { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo del pasajero es requerido.")]
        [EmailAddress(ErrorMessage = "El correo del pasajero no tiene un formato valido.")]
        [StringLength(50, ErrorMessage = "El correo del pasajero no puede superar los 50 caracteres.")]
        public string EmailPassenger { get; set; } = string.Empty;

        [Range(1, 999999999999999, ErrorMessage = "El telefono del pasajero debe ser un numero valido.")]
        public long? Telephone { get; set; }

        [Required(ErrorMessage = "El pais del pasaporte es requerido.")]
        [StringLength(50, ErrorMessage = "El pais del pasaporte no puede superar los 50 caracteres.")]
        public string Country { get; set; } = string.Empty;
    }
}
