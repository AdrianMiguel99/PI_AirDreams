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
<<<<<<< HEAD
            return Ok(users);
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public ActionResult<UserDTO> GetById(int id)
        {
            // TODO : User service and repository to get a user by id
            var user = userService.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
=======
            if (users == null || users.Count == 0)
            {
                return NotFound("No users found");
            }
            return Ok(users);
>>>>>>> dde80bb (Add User Controller and DTO)
        }

    }
}