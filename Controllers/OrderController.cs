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
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetAllOrders()
        {
            var orders = _orderService.GetAllOrders();
            return Ok(orders);
        }

        // Get order by ID
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
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
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetOrdersByCustomerId(string customerId)
        {
            var orders = _orderService.GetOrdersByCustomerId(customerId);
            return Ok(orders);
        }

        // Create order
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult CreateOrder(OrderModel orderModel)
        {
            var order = _orderService.CreateOrder(orderModel);
            return Ok(order);
        }

        // Update order
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult UpdateOrder(string id, OrderModel orderModel)
        {
            _orderService.UpdateOrder(id, orderModel);
            return Ok(orderModel);
        }

        // Delete order
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult DeleteOrder(string id)
        {
            _orderService.DeleteOrder(id);
            return Ok(new { Message = "Order deleted successfully." });
        }

        // update order status
        [HttpPut("{id}/{status}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult UpdateOrderStatus(string id, string status)
        {
            _orderService.UpdateOrderStatus(id, status);
            return Ok();
        }

        // update vendor order status
        [HttpPut("{orderId}/vendor/{vendorId}/{status}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult UpdateVendorOrderStatus(string orderId, string vendorId, string status)
        {
            _orderService.UpdateVendorOrderStatus(orderId, vendorId, status);
            return Ok();
        }
    }
}