using AirDreams.API.DTOs;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public ActionResult<List<UserDTO>> GetAll()
        {
            var users = userService.GetAll();
            return Ok(users);
        }

        [HttpGet("search")]
        public ActionResult<List<UserDTO>> Search([FromQuery] string searchTerm)
        {
            var users = userService.Search(searchTerm);
            return Ok(users);
        }

    }
}