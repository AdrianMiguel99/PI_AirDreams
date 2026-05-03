using Microsoft.AspNetCore.Mvc;
using AirDreams.API.Models;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
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
            
            // guardar en BD
            // generar token
            // enviar correo real

            return Ok("Usuario registrado. Correo enviado.");
        }
    }
}
