using Microsoft.AspNetCore.Mvc;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EADBackend.Services;

namespace EADBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CartModel>), 200)]
        public IActionResult GetAllCarts()
        {
            return Ok(_cartService.GetAllCarts());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CartModel), 200)]
        public IActionResult GetCartById(string id)
        {
            var cart = _cartService.GetCartById(id);
            if (cart == null)
            {
                return NotFound(new { status = 404, error = "Cart not found." });
            }

            return Ok(cart);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult CreateCart([FromBody] CartModel cartModel)
        {
            try
            {
                _cartService.CreateCart(cartModel);
                return Ok(new { status = 200, added = new { Message = "Cart created successfully." } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 400, error = "An unexpected error occurred.", exception = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult UpdateCart(string id, [FromBody] CartModel cartModel)
        {
            try
            {
                _cartService.UpdateCart(id, cartModel);
                return Ok(new { status = 200, updated = new { Message = "Cart updated successfully." } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 400, error = "An unexpected error occurred.", exception = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult DeleteCart(string id)
        {
            try
            {
                _cartService.DeleteCart(id);
                return Ok(new { status = 200, deleted = new { Message = "Cart deleted successfully." } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 400, error = "An unexpected error occurred.", exception = ex.Message });
            }
        }
    }
}