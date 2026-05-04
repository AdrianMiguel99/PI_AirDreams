using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}