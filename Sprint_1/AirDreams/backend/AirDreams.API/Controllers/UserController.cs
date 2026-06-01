using AirDreams.API.DTOs;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AirDreams.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> GetAll()
        {
            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(currentUserEmail))
            {
                return Unauthorized(new
                {
                    message = "No se pudo identificar al usuario"
                });
            }

            var users = await _userService.GetAll(currentUserEmail);

            return Ok(users);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<UserDTO>>> Search([FromQuery] string searchTerm)
        {
            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(currentUserEmail))
            {
                return Unauthorized(new
                {
                    message = "No se pudo identificar al usuario"
                });
            }

            var users = await _userService.Search(searchTerm, currentUserEmail);

            return Ok(users);
        }

        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateUser(
            byte employeeId,
            [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                var currentUserEmail =
                    User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(currentUserEmail))
                {
                    return Unauthorized(new
                    {
                        message = "No se pudo identificar al usuario"
                    });
                }

                var result = await _userService.UpdateUserAsync(
                    employeeId,
                    updateDto,
                    currentUserEmail
                );

                if (!result)
                {
                    return NotFound(new
                    {
                        message = $"Usuario con ID {employeeId} no encontrado"
                    });
                }

                return Ok(new
                {
                    message = "Usuario actualizado correctamente"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error interno del servidor",
                    detail = ex.Message
                });
            }
        }
    }
}