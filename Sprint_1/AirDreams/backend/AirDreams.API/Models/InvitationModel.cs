using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.Models
{
    public class InvitationModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Correo { get; set; } = string.Empty;
        
        [Required]
        public string TipoUsuario { get; set; } = string.Empty; // Administrador u Operario
    }
}