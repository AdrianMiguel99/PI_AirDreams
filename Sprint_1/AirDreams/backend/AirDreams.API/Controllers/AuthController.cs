using AirDreams.API.Models;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
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

            return Ok(new { 
                isValid = true, 
                email = result.Email,
                tipoUsuario = result.TipoUsuario,
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

            if (result.success)
            {
                // Aquí JWT token para mantener la sesión, pero por ahora solo devolvemos datos básicos
                return Ok(new { 
                    message = result.message,
                    user = new 
                    {
                        result.user.Id,
                        result.user.NombreCompleto,
                        result.user.Correo,
                        result.user.TipoUsuario
                    }
                });
            }

            return Unauthorized(new { message = "Correo o contraseña incorrecta" });
        }
    }
}