using System;

namespace AirDreams.API.Models
{
    public class InvitationData
    {
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
        
        public DateTime ExpiryDate { get; set; }
    }
}