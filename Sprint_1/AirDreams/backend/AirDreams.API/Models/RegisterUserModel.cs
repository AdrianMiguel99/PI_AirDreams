namespace AirDreams.API.Models
{
    public class RegisterUserModel
    {
        public string NombreCompleto { get; set; } = string.Empty;

        public string TipoUsuario { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Cedula { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }

        public bool MustChangePassword { get; set; } = true; // Nuevo: debe cambiar contraseña
        
        public string? TemporaryPassword { get; set; } // Contraseña temporal (no se guarda en BD)
    }
}