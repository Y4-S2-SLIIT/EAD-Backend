using Microsoft.AspNetCore.Mvc;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EADBackend.Services;

namespace EADBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IJwtService _jwtService;

        public CustomerController(ICustomerService customerService, IJwtService jwtService)
        {
            _customerService = customerService;
            _jwtService = jwtService;
        }

        // Login
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (!_customerService.ValidateCustomer(loginModel.Username, loginModel.Password))
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

        // CRUD Operations for CustomerModel

        // Create a new customer
        [HttpPost("register")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult Register([FromBody] CustomerModel customerModel)
        {
            try
            {
                if (_customerService.IsEmailTaken(customerModel.Email))
                {
                    return BadRequest(new { status = 400, error = "Email is already in use." });
                }

                if (_customerService.IsUsernameTaken(customerModel.Username))
                {
                    return BadRequest(new { status = 400, error = "Username is already in use." });
                }

                _customerService.CreateCustomer(customerModel);
                return Ok(new { status = 200, added = new { Message = "Customer created successfully." } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 400, error = ex.Message });
            }
        }

        // Get all customers
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerModel>), 200)]
        [Authorize]
        public IActionResult GetAllCustomers()
        {
            return Ok(_customerService.GetAllCustomers());
        }

        // Get a customer by ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerModel), 200)]
        [Authorize]
        public IActionResult GetCustomerById(string id)
        {
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound(new { status = 404, error = "Customer not found." });
            }

            return Ok(customer);
        }

        // Update a customer
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [Authorize]
        public IActionResult UpdateCustomer(string id, [FromBody] CustomerModel customerModel)
        {
            try
            {
                _customerService.UpdateCustomer(id, customerModel);
                return Ok(new { status = 200, added = new { Message = "Customer updated successfully." } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 400, error = ex.Message });
            }
        }

        // Delete a customer
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [Authorize]
        public IActionResult DeleteCustomer(string id)
        {
            try
            {
                _customerService.DeleteCustomer(id);
                return Ok(new { status = 200, added = new { Message = "Customer deleted successfully." } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 400, error = ex.Message });
            }
        }
    }
}
