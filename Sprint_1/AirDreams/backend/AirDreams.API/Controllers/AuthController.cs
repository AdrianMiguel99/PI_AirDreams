using Microsoft.AspNetCore.Mvc;
using AirDreams.API.Models;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService userService;

        public AuthController()
        {
            userService = new UserService();
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (login.Correo == "admin@air.com" && login.Password == "1234")
            {
                return Ok("Login correcto");
            }

            return Unauthorized("Correo o contraseña incorrectos");
        }
        // REGISTER
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel register)
        {
            if (
                string.IsNullOrEmpty(register.NombreCompleto) ||
                string.IsNullOrEmpty(register.TipoUsuario) ||
                string.IsNullOrEmpty(register.Correo) ||
                string.IsNullOrEmpty(register.Cedula)
            )
            {
                return BadRequest("Todos los campos son obligatorios");
            }
            
            var user = await userServuce
            // guardar en BD
            // generar token
            // enviar correo real

            return Ok("Usuario registrado. Correo enviado.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            try
            {
                var userId = await _registerService.Execute(
                    req.Nombre,
                    req.Email,
                    req.Cedula,
                    req.TipoUsuario
                );

            // await _sendActivationEmailService.Execute(userId, req.Email);

                return Ok("Usuario registrado. Revisa tu correo.");
            }
            catch (Exception ex)
            {
                if (ex.Message == "El correo ya está registrado")
                    return BadRequest("El correo ya está en uso");

                return StatusCode(500, "Error interno");
            }
        }
    }
}
