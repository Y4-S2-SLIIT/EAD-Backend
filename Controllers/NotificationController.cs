using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EADBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        private readonly string notFoundMessage = "Notification not found";

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Get all notifications
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<NotificationModel>), 200)]
        public IActionResult GetAllNotifications()
        {
            var notifications = _notificationService.GetAllNotifications();
            return Ok(notifications);
        }

        // Get notification by ID
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(NotificationModel), 200)]
        public IActionResult GetNotificationById(string id)
        {
            var notification = _notificationService.GetNotificationById(id);
            if (notification == null)
                return NotFound(notFoundMessage);
            return Ok(notification);
        }

        // Get notifications by vendor ID
        [HttpGet("vendor/{vendorId}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<NotificationModel>), 200)]
        public IActionResult GetNotificationsByVendorId(string vendorId)
        {
            var notifications = _notificationService.GetNotificationsByVendorId(vendorId);
            return Ok(notifications);
        }

        // Create notification
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(NotificationModel), 200)]
        public IActionResult CreateNotification(NotificationModel notificationModel)
        {
            _notificationService.CreateNotification(notificationModel);
            return Ok(notificationModel);
        }

        // Update notification
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        public IActionResult UpdateNotification(string id, NotificationModel notificationModel)
        {
            _notificationService.UpdateNotification(id, notificationModel);
            return Ok();
        }

        // Delete notification
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        public IActionResult DeleteNotification(string id)
        {
            _notificationService.DeleteNotification(id);
            return Ok();
        }
    }
}