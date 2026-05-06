namespace AirDreams.API.Models
{
    public class RegisterUserModel
    {
        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string id { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }

        public bool MustChangePassword { get; set; } = true; 
        
        public string? TemporaryPassword { get; set; } 
    }
}