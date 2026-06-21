using AirDreams.API.Models;
using AirDreams.API.Repositories;
using AirDreams.API.DTOs;
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

        public async Task<List<UserDTO>> GetAll(string currentUserEmail)
        {
            var currentUser = await _userRepository.GetUserByEmail(currentUserEmail);

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("Usuario no encontrado");
            }

            if (currentUser.Role == "Admin")
            {
                return _userRepository.GetAll();
            }

            var ownUser = await _userRepository.GetUserById(currentUser.Id);

            return ownUser != null
                ? new List<UserDTO>
                {
                    new UserDTO
                    {
                        Id = ownUser.Id,
                        FullName = ownUser.FullName,
                        Email = ownUser.Email,
                        Role = ownUser.Role
                    }
                }
                : new List<UserDTO>();
        }

        public async Task<List<UserDTO>> Search(string searchTerm, string currentUserEmail)
        {
            var currentUser = await _userRepository.GetUserByEmail(currentUserEmail);

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("Usuario no encontrado");
            }

            if (currentUser.Role == "Admin")
            {
                return _userRepository.Search(searchTerm);
            }

            var ownUser = await _userRepository.GetUserById(currentUser.Id);

            if (ownUser == null)
            {
                return new List<UserDTO>();
            }

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<UserDTO>
                {
                    new UserDTO
                    {
                        Id = ownUser.Id,
                        FullName = ownUser.FullName,
                        Email = ownUser.Email,
                        Role = ownUser.Role
                    }
                };
            }

            bool matches =
                ownUser.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || ownUser.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);

            if (!matches)
            {
                return new List<UserDTO>();
            }

            return new List<UserDTO>
            {
                new UserDTO
                {
                    Id = ownUser.Id,
                    FullName = ownUser.FullName,
                    Email = ownUser.Email,
                    Role = ownUser.Role
                }
            };
        }

        public async Task<(bool success, string message)> SendInvitation(InvitationModel model)
        {
            try
            {
                var exists = await _userRepository.ExistsByEmail(model.Email);
                if (exists)
                {
                    return (false, "El Email ya tiene una invitación pendiente o ya está registrado");
                }

                var token = GenerateUniqueToken();
                var expiryHours = _configuration.GetValue<int>("AppSettings:InvitationExpiryHours", 48);
                var expiryDate = DateTime.UtcNow.AddHours(expiryHours);

                var userId = await _userRepository.CreateInvitation(model.Email, model.Role, token, expiryDate);

                if (userId > 0)
                {
                    await _emailService.SendInvitationEmail(model.Email, token, model.Role);
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

                if (!IsValidCostaRicanId(model.id))
                {
                    return (false, "Formato de cédula inválido. Debe ser 0-0000-0000");
                }

                if (!IsValidName(model.FullName))
                {
                    return (false, "El nombre no puede contener números");
                }

                if (!IsStrongPassword(model.Password))
                {
                    return (false, "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un carácter especial");
                }

                var passwordHash = HashPassword(model.Password);

                var success = await _userRepository.CompleteRegistration(
                    model.Token, 
                    model.FullName, 
                    model.id, 
                    passwordHash
                );

                if (success)
                {
                    await _emailService.SendWelcomeEmail(invitation.Email, model.FullName);
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
                var user = await _userRepository.GetUserByEmail(model.Email);

                if (user == null)
                {
                    return (false, "Email o contraseña incorrecta", null);
                }

                if (!user.IsActive)
                {
                    return (false, "Debes completar tu registro usando el link que recibiste por Email", null);
                }

                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    return (false, "Debes establecer una contraseña primero", null);
                }

                if (!VerifyPassword(model.Password, user.PasswordHash))
                {
                    return (false, "Email o contraseña incorrecta", null);
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

                string role = "";
                var prop = invitation.GetType().GetProperty("Role");
                if (prop != null)
                    role = prop.GetValue(invitation)?.ToString() ?? "";
                else
                    role = "Employee";

                return new InvitationValidationResult
                {
                    IsValid = true,
                    Email = invitation.Email,
                    Role = role,
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

        private bool IsValidCostaRicanId(string id)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^\d{1}-\d{4}-\d{4}$");
            return regex.IsMatch(id);
        }

        private bool IsValidName(string nombre)
        {
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

        public async Task<bool> UpdateUserAsync(byte employeeId, UpdateUserDto updateDto, string currentUserEmail)
        {
            var currentUserId = await _userRepository.GetCurrentUserIdFromEmailAsync(currentUserEmail);
            var currentUser = await _userRepository.GetAirlineEmployeeByIdAsync(currentUserId);
            
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("Usuario no encontrado");
            }
            
            var targetUser = await _userRepository.GetAirlineEmployeeByIdAsync(employeeId);
            
            if (targetUser == null)
            {
                return false;
            }
            
            bool isAdmin = currentUser.IsAdmin;
            bool isEditingSelf = currentUserId == employeeId;
            
            if (!isAdmin && !isEditingSelf)
            {
                throw new UnauthorizedAccessException("No tienes permiso para editar otros usuarios");
            }
            
            string? firstName = updateDto.FirstName;
            string? lastName = updateDto.LastName;
            bool? isAdminUpdate = isAdmin ? updateDto.IsAdmin : null;
            bool? isOperatorUpdate = isAdmin ? updateDto.IsOperator : null;
            bool? isActiveUpdate = isAdmin ? updateDto.IsActive : null;
            
            await _userRepository.UpdateAirlineEmployeeAsync(employeeId, firstName, lastName, isAdminUpdate, isOperatorUpdate);
            await _userRepository.UpdateInternalUserActiveStatusAsync(employeeId, isActiveUpdate);
            
            return true;
        }

        public async Task<bool> DeleteUserAsync(byte employeeId, string currentUserEmail)
        {
            var currentUserId =
                await _userRepository.GetCurrentUserIdFromEmailAsync(currentUserEmail);

            var currentUser =
                await _userRepository.GetAirlineEmployeeByIdAsync(currentUserId);

            var targetUser =
                await _userRepository.GetAirlineEmployeeByIdAsync(employeeId);

            if (currentUser == null)
                throw new UnauthorizedAccessException("Usuario actual no encontrado.");

            if (targetUser == null)
                throw new KeyNotFoundException("Usuario a eliminar no encontrado.");

            if (!currentUser.IsAdmin)
                throw new UnauthorizedAccessException("Solo un administrador puede eliminar usuarios.");

            if (currentUserId == employeeId)
                throw new InvalidOperationException("No puede eliminarse a sí mismo.");

            if (targetUser.EmailInternalUser == "admin3@air.com")
                throw new InvalidOperationException("No se puede eliminar el administrador principal.");

            await _userRepository.SoftDeleteUserAsync(employeeId);

            return true;
        }
    }
}


