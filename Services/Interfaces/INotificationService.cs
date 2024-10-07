using EADBackend.Models;

namespace EADBackend.Services.Interfaces;

public interface INotificationService
{
    IEnumerable<NotificationModel> GetAllNotifications();
    NotificationModel GetNotificationById(string id);
    void CreateNotification(NotificationModel notificationModel);
    void UpdateNotification(string id, NotificationModel notificationModel);
    void DeleteNotification(string id);
    IEnumerable<NotificationModel> GetNotificationsByVendorId(string vendorId);
}