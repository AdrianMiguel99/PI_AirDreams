using AirDreams.API.DTOs;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        // GET: api/User for list
        [HttpGet]
        public ActionResult<List<UserDTO>> GetAll()
        {
            // TODO : User service and repository to get all users
            var users = userService.GetAllUsers();
            if (users == null || users.Count == 0)
            {
                return NotFound("No users found");
            }
            return Ok(users);
        }

    }
}