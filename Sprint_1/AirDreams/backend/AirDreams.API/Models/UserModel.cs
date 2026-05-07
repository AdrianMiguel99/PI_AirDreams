using System;

namespace AirDreams.API.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string? PasswordHash { get; set; }

        public string? InvitationToken { get; set; }

        public DateTime? InvitationExpiryDate { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}