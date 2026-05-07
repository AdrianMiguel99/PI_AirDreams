using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.Models
{
    public class InvitationModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Formato de Email inválido")]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}