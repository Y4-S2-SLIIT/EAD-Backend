using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace EADBackend.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<NotificationModel> _notificationCollection;

        public NotificationService(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _notificationCollection = database.GetCollection<NotificationModel>("Notifications");
        }

        public IEnumerable<NotificationModel> GetAllNotifications()
        {
            return _notificationCollection.Find(notification => true).ToList();
        }

        public NotificationModel GetNotificationById(string id)
        {
            return _notificationCollection.Find(notification => notification.Id == id).FirstOrDefault();
        }

        public void CreateNotification(NotificationModel notificationModel)
        {
            _notificationCollection.InsertOne(notificationModel);
        }

        public void UpdateNotification(string id, NotificationModel notificationModel)
        {
            _notificationCollection.ReplaceOne(notification => notification.Id == id, notificationModel);
        }

        public void DeleteNotification(string id)
        {
            var filter = Builders<NotificationModel>.Filter.Eq(notification => notification.Id, id);
            _notificationCollection.DeleteOne(filter);
        }

        public IEnumerable<NotificationModel> GetNotificationsByVendorId(string vendorId)
        {
            return _notificationCollection.Find(notification => notification.VendorId == vendorId).ToList();
        }
    }
}