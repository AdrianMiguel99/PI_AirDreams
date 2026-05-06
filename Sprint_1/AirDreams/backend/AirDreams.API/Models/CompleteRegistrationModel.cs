using System.ComponentModel.DataAnnotations;

namespace AirDreams.API.Models
{
    public class CompleteRegistrationModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}$", 
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un carácter especial")]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre no puede contener números")]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [RegularExpression(@"^\d{1}-\d{4}-\d{4}$", ErrorMessage = "Formato de cédula inválido (ej: 0-0000-0000)")]
        public string id { get; set; } = string.Empty;
    }
}