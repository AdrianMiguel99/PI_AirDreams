using System;

namespace AirDreams.API.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Cedula { get; set; } = string.Empty;

        public string TipoUsuario { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string? PasswordHash { get; set; }

        public string? InvitationToken { get; set; }

        public DateTime? InvitationExpiryDate { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}