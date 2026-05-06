using System;
using AirDreams.API.DTOs;

namespace AirDreams.API.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        
        public string Token { get; set; } = string.Empty;
        
        public DateTime ExpiresAt { get; set; }
        
        public string Message { get; set; } = string.Empty;
        
        public UserDTO? User { get; set; } 
}