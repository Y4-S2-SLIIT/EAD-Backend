using Microsoft.AspNetCore.Mvc;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EADBackend.Services;

namespace EADBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        // Login
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (!_userService.ValidateUser(loginModel.Username, loginModel.Password))
            {
                return BadRequest(new { status = 400, error = "Invalid credentials." });
            }

            var token = _jwtService.GenerateToken(loginModel.Username);
            return Ok(new { status = 200, added = new { Token = token } });
        }

        // Secure Data
        [HttpGet("secure-data")]
        [ProducesResponseType(typeof(string), 200)]
        [Authorize]
        public IActionResult GetSecureData()
        {
            return Ok(new { status = "200", added = new { Data = "This is secure data only for authenticated users." } });
        }

        // CRUD Operations for UserModel

        // Create a new user
        [HttpPost("register")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult Register([FromBody] UserModel userModel)
        {
            try
            {
                if (_userService.IsEmailTaken(userModel.Email))
                {
                    return BadRequest(new { status = 400, error = "Email is already in use." });
                }

                if (_userService.IsUsernameTaken(userModel.Username))
                {
                    return BadRequest(new { status = 400, error = "Username is already taken." });
                }

                _userService.CreateUser(userModel);
                return Ok(new { status = 200, added = new { Message = "User registered successfully." } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 400, error = "An unexpected error occurred.", exception = ex.Message });
            }
        }

        // Get a user by Id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserModel), 200)]
        [Authorize]
        public IActionResult GetUser(string id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { status = 400, error = "User not found." });
            }

            return Ok(new { status = 200, data = user });
        }

        // Get all users
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<UserModel>), 200)]
        [Authorize]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(new { status = 200, data = users });
        }

        // Update a user
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [Authorize]
        public IActionResult UpdateUser(string id, [FromBody] UserModel userModel)
        {
            var existingUser = _userService.GetUserById(id);
            if (existingUser == null)
            {
                return NotFound(new { status = 400, error = "User not found." });
            }

            _userService.UpdateUser(id, userModel);
            return Ok(new { status = 200, message = "User updated successfully." });
        }

        // Delete a user
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [Authorize]
        public IActionResult DeleteUser(string id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { status = 400, error = "User not found." });
            }

            _userService.DeleteUser(id);
            return Ok(new { status = 200, message = "User deleted successfully." });
        }
    }
}