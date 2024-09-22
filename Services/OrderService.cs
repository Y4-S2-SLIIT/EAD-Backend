using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EADBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<OrderModel> _orderCollection;

        public OrderService(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _orderCollection = database.GetCollection<OrderModel>("Orders");
        }

        public OrderModel CreateOrder(OrderModel orderModel)
        {
            _orderCollection.InsertOne(orderModel);
            return orderModel;
        }

        public void DeleteOrder(string id)
        {
            var filter = Builders<OrderModel>.Filter.Eq(o => o.Id, id);
            _orderCollection.DeleteOne(filter);
        }

        public IEnumerable<OrderModel> GetAllOrders()
        {
            return _orderCollection.Find(_ => true).ToList();
        }

        public OrderModel GetOrderById(string id)
        {
            return _orderCollection.Find(o => o.Id == id).FirstOrDefault();
        }

        public IEnumerable<OrderModel> GetOrdersByCustomerId(string customerId)
        {
            return _orderCollection.Find(o => o.CustomerId == customerId).ToList();
        }

        public IEnumerable<OrderModel> GetOrdersByVendorId(string vendorId)
        {
            var filter = Builders<OrderModel>.Filter.ElemMatch(o => o.Items,
        Builders<VendorOrderItems>.Filter.Eq(v => v.VenderId, vendorId));
            return _orderCollection.Find(filter).ToList();
        }

        public void UpdateOrder(string id, OrderModel orderModel)
        {
            var filter = Builders<OrderModel>.Filter.Eq(o => o.Id, id);
            _orderCollection.ReplaceOne(filter, orderModel);
        }
    }
}