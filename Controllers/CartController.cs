// IT21105302, Fernando U.S.L, CartController
using Microsoft.AspNetCore.Mvc;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;

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

        // Get all carts
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CartModel>), 200)]
        public IActionResult GetAllCarts()
        {
            return Ok(_cartService.GetAllCarts());
        }

        // Get cart by id
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

        // Get cart by userId
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(CartModel), 200)]
        public IActionResult GetCartByUserId(string userId)
        {
            var cart = _cartService.GetCartByUserId(userId);
            if (cart == null)
            {
                return NotFound(new { status = 404, error = "Cart not found." });
            }

            return Ok(cart);
        }

        // Create a new cart
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

        // Add item to cart
        [HttpPost("add")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult AddItemToCart([FromBody] CartItemModel cartItemModel, string userId)
        {
            try
            {
                _cartService.AddItemToCart(userId, cartItemModel);
                return Ok(new { status = 200, message = "Item added to cart successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 400, error = "An unexpected error occurred.", exception = ex.Message });
            }
        }

        // Remove item from cart
        [HttpDelete("remove/{productId}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult RemoveItemFromCart(string productId, string userId)
        {
            try
            {
                _cartService.RemoveItemFromCart(userId, productId);
                return Ok(new { status = 200, message = "Item removed from cart successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 400, error = "An unexpected error occurred.", exception = ex.Message });
            }
        }

        // Update item quantity in cart
        [HttpPut("update-quantity/{productId}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult UpdateItemQuantity(string productId, [FromBody] UpdateQuantityModel newQuantity, string userId)
        {
            try
            {
                _cartService.UpdateItemQuantity(userId, productId, newQuantity.Quantity);
                return Ok(new { status = 200, message = "Item quantity updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 400, error = "An unexpected error occurred.", exception = ex.Message });
            }
        }

        // Update cart by id
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

        // Delete cart by id
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
