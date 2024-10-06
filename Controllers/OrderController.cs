using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EADBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        private readonly string notFoundMessage = "Order not found";

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Get all orders
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<OrderModel>), 200)]
        public IActionResult GetAllOrders()
        {
            var orders = _orderService.GetAllOrders();
            return Ok(orders);
        }

        // Get order by ID
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(OrderModel), 200)]
        public IActionResult GetOrderById(string id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound(notFoundMessage);
            return Ok(order);
        }

        // Get orders by customer ID
        [HttpGet("customer/{customerId}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<OrderModel>), 200)]
        public IActionResult GetOrdersByCustomerId(string customerId)
        {
            var orders = _orderService.GetOrdersByCustomerId(customerId);
            return Ok(orders);
        }

        // Create order
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(OrderModel), 200)]
        public IActionResult CreateOrder(OrderModel orderModel)
        {
            var order = _orderService.CreateOrder(orderModel);
            return Ok(order);
        }

        // Update order
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        public IActionResult UpdateOrder(string id, OrderModel orderModel)
        {
            _orderService.UpdateOrder(id, orderModel);
            return NoContent(); // 204 No Content
        }

        // Delete order
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        public IActionResult DeleteOrder(string id)
        {
            _orderService.DeleteOrder(id);
            return NoContent(); // 204 No Content
        }

        // Get orders by vendor ID
        [HttpGet("vendor/{vendorId}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<OrderModel>), 200)]
        public IActionResult GetOrdersByVendorId(string vendorId)
        {
            var orders = _orderService.GetOrdersByVendorId(vendorId);
            if (orders == null || !orders.Any())
                return NotFound("No orders found for this vendor.");
            return Ok(orders);
        }
    }
}
