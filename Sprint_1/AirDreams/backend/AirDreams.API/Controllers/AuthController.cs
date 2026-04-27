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
    }
}