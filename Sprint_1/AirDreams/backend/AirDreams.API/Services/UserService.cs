using AirDreams.API.Models;
using AirDreams.API.Repositories;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AirDreams.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IEmailService emailService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<(bool success, string message)> SendInvitation(InvitationModel model)
        {
            try
            {
                // Validar que el correo no exista ya (ni como invitación pendiente ni como usuario activo)
                var exists = await _userRepository.ExistsByEmail(model.Correo);
                if (exists)
                {
                    return (false, "El correo ya tiene una invitación pendiente o ya está registrado");
                }

                var token = GenerateUniqueToken();
                var expiryHours = _configuration.GetValue<int>("AppSettings:InvitationExpiryHours", 48);
                var expiryDate = DateTime.UtcNow.AddHours(expiryHours);

                // Crear invitación en BD
                var userId = await _userRepository.CreateInvitation(model.Correo, model.TipoUsuario, token, expiryDate);

                if (userId > 0)
                {
                    await _emailService.SendInvitationEmail(model.Correo, token, model.TipoUsuario);
                    return (true, "Invitación enviada exitosamente");
                }

                return (false, "Error al crear la invitación");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> CompleteRegistration(CompleteRegistrationModel model)
        {
            try
            {
                var invitation = await _userRepository.GetInvitationByToken(model.Token);
                if (invitation == null)
                {
                    return (false, "Token inválido");
                }

                if (invitation.ExpiryDate < DateTime.UtcNow)
                {
                    return (false, "La invitación ha expirado. Solicita una nueva invitación");
                }

                if (!IsValidCostaRicanId(model.Cedula))
                {
                    return (false, "Formato de cédula inválido. Debe ser 0-0000-0000");
                }

                if (!IsValidName(model.NombreCompleto))
                {
                    return (false, "El nombre no puede contener números");
                }

                if (!IsStrongPassword(model.Password))
                {
                    return (false, "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un carácter especial");
                }

                // Encriptar contraseña
                var passwordHash = HashPassword(model.Password);

                var success = await _userRepository.CompleteRegistration(
                    model.Token, 
                    model.NombreCompleto, 
                    model.Cedula, 
                    passwordHash
                );

                if (success)
                {
                    await _emailService.SendWelcomeEmail(invitation.Email, model.NombreCompleto);
                    return (true, "Registro completado exitosamente. Ya puedes iniciar sesión");
                }

                return (false, "Error al completar el registro");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool success, string message, UserModel? user)> Login(LoginModel model)
        {
            try
            {
                var user = await _userRepository.GetUserByEmail(model.Correo);

                if (user == null)
                {
                    return (false, "Correo o contraseña incorrecta", null);
                }

                if (!user.IsActive)
                {
                    return (false, "Debes completar tu registro usando el link que recibiste por correo", null);
                }

                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    return (false, "Debes establecer una contraseña primero", null);
                }

                if (!VerifyPassword(model.Password, user.PasswordHash))
                {
                    return (false, "Correo o contraseña incorrecta", null);
                }

                return (true, "Login exitoso", user);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<InvitationValidationResult?> ValidateInvitationToken(string token)
        {
            try
            {
                var invitation = await _userRepository.GetInvitationByToken(token);
                
                if (invitation == null)
                {
                    return new InvitationValidationResult
                    {
                        IsValid = false,
                        Message = "Token inválido"
                    };
                }

                if (invitation.ExpiryDate < DateTime.UtcNow)
                {
                    return new InvitationValidationResult
                    {
                        IsValid = false,
                        Message = "La invitación ha expirado"
                    };
                }

                return new InvitationValidationResult
                {
                    IsValid = true,
                    Email = invitation.Email,
                    TipoUsuario = invitation.TipoUsuario,
                    Message = "Token válido"
                };
            }
            catch (Exception)
            {
                return new InvitationValidationResult
                {
                    IsValid = false,
                    Message = "Error al validar el token"
                };
            }
        }

        private string GenerateUniqueToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        private bool IsValidCostaRicanId(string cedula)
        {
            // Formato: 0-0000-0000
            var regex = new System.Text.RegularExpressions.Regex(@"^\d{1}-\d{4}-\d{4}$");
            return regex.IsMatch(cedula);
        }

        private bool IsValidName(string nombre)
        {
            // Solo letras, espacios, acentos y ñ
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$");
            return regex.IsMatch(nombre) && nombre.Length <= 50;
        }

        private bool IsStrongPassword(string password)
        {
            // Requisitos:
            // - Al menos 8 caracteres
            // - Al menos una mayúscula (A-Z)
            // - Al menos un número (0-9)
            // - Al menos un carácter especial ($ @ $ ! % * ? &)
            
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;
            
            bool hasUpper = false;
            bool hasNumber = false;
            bool hasSpecial = false;
            
            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsDigit(c)) hasNumber = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }
            
            return hasUpper && hasNumber && hasSpecial;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
    }
}