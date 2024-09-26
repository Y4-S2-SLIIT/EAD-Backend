using Microsoft.AspNetCore.Mvc;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EADBackend.Services;

namespace EADBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly IJwtService _jwtService;

        public VendorController(IVendorService vendorService, IJwtService jwtService)
        {
            _vendorService = vendorService;
            _jwtService = jwtService;
        }

        // Login
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var vendorId = _vendorService.ValidateVendor(loginModel.Username, loginModel.Password);

            // Check if validation failed (vendorId is null)
            if (vendorId == null)
            {
                return BadRequest(new { status = 400, error = "Invalid credentials." });
            }
            var token = _jwtService.GenerateToken(vendorId);

            return Ok(new { status = 200, Token = token, VendorId = vendorId });
        }

        // Create a new vendor
        [HttpPost("register")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult Register([FromBody] VendorModel vendorModel)
        {
            try
            {
                if (_vendorService.IsEmailTaken(vendorModel.Email))
                {
                    return BadRequest(new { status = 400, error = "Email is already in use." });
                }

                if (_vendorService.IsUsernameTaken(vendorModel.Username))
                {
                    return BadRequest(new { status = 400, error = "Username is already taken." });
                }

                _vendorService.CreateVendor(vendorModel);
                return Ok(new { status = 200, added = new { Message = "Vendor registered successfully." } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 400, error = "An unexpected error occurred.", exception = ex.Message });
            }
        }

        // Get all vendors
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VendorModel>), 200)]
        [Authorize]
        public IActionResult GetAllVendors()
        {
            return Ok(_vendorService.GetAllVendors());
        }

        // Get a vendor by Id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VendorModel), 200)]
        [Authorize]
        public IActionResult GetVendor(string id)
        {
            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null)
            {
                return NotFound(new { status = 400, error = "Vendor not found." });
            }

            return Ok(new { status = 200, data = vendor });
        }

        // Update a vendor
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult UpdateVendor(string id, [FromBody] VendorModel vendorModel)
        {
            _vendorService.UpdateVendor(id, vendorModel);
            return Ok(new { status = 200, updated = new { Message = "Vendor updated successfully." } });
        }

        // Delete a vendor
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult DeleteVendor(string id)
        {
            _vendorService.DeleteVendor(id);
            return Ok(new { status = 200, deleted = new { Message = "Vendor deleted successfully." } });
        }

        // Verify a vendor
        [HttpPut("verify/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult VerifyVendor(string id)
        {
            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null)
            {
                return NotFound(new { status = 400, error = "Vendor not found." });
            }

            vendor.IsVerified = true;
            _vendorService.UpdateVendor(id, vendor);
            return Ok(new { status = 200, updated = new { Message = "Vendor verified successfully." } });
        }

        // Deactivate a vendor
        [HttpPut("deactivate/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult DeactivateVendor(string id)
        {
            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null)
            {
                return NotFound(new { status = 400, error = "Vendor not found." });
            }

            vendor.IsDeactivated = true;
            _vendorService.UpdateVendor(id, vendor);
            return Ok(new { status = 200, updated = new { Message = "Vendor deactivated successfully." } });
        }

        // Activate a vendor
        [HttpPut("activate/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult ActivateVendor(string id)
        {
            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null)
            {
                return NotFound(new { status = 400, error = "Vendor not found." });
            }

            vendor.IsDeactivated = false;
            _vendorService.UpdateVendor(id, vendor);
            return Ok(new { status = 200, updated = new { Message = "Vendor activated successfully." } });
        }
    }
}