using AirDreams.API.Models;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;  // ← NUEVO

        public AuthController(IUserService userService, IJwtService jwtService)  // ← Inyectar JwtService
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        // Endpoint para administrador envíe invitación (solo Administradores)
        [HttpPost("invite")]
        public async Task<IActionResult> SendInvitation([FromBody] InvitationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values });
            }

            var result = await _userService.SendInvitation(model);

            if (result.success)
            {
                return Ok(new { message = result.message });
            }

            return BadRequest(new { message = result.message });
        }

        [HttpGet("validate-invitation")]
        public async Task<IActionResult> ValidateInvitation([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token requerido" });
            }

            var result = await _userService.ValidateInvitationToken(token);

            if (result == null || !result.IsValid)
            {
                return BadRequest(new { message = result?.Message ?? "Token inválido" });
            }

            return Ok(new 
            { 
                isValid = true, 
                email = result.Email,
                Role = result.Role,
                message = result.Message 
            });
        }

        // Endpoint para completar registro (usuario invitado)
        [HttpPost("complete-registration")]
        public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values });
            }

            var result = await _userService.CompleteRegistration(model);

            if (result.success)
            {
                return Ok(new { message = result.message });
            }

            return BadRequest(new { message = result.message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values });
            }

            var result = await _userService.Login(model);

            if (result.success && result.user != null)
            {
                // Generar token JWT directamente con UserModel
                var token = _jwtService.GenerateToken(result.user);
                
                // Respuesta con token
                return Ok(new 
                { 
                    success = true,
                    token = token,
                    expiresAt = DateTime.UtcNow.AddMinutes(60),
                    message = result.message,
                    user = new 
                    {
                        result.user.Id,
                        result.user.FullName,
                        result.user.Email,
                        result.user.Role
                    }
                });
            }

            return Unauthorized(new { message = "Email o contraseña incorrecta" });
        }
    }
}