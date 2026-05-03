using AirDreams.API.DTOs;
using AirDreams.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IService<UserDTO> userService;

        public UserController(IService<UserDTO> userService)
        {
            this.userService = userService;
        }

        // GET: api/User for list
        [HttpGet]
        public ActionResult<List<UserDTO>> GetAll()
        {
            // TODO : User service and repository to get all users
            var users = userService.GetAll();
            return Ok(users);
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public ActionResult<UserDTO> GetById(string id)
        {
            // TODO : User service and repository to get a user by id
            var user = userService.GetById(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }

    }
}