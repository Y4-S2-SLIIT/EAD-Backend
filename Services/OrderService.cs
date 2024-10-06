using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace EADBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<OrderModel> _orderCollection;
        private readonly IProductService _productService; // Injecting ProductService

        public OrderService(IMongoClient client, IOptions<MongoDbSettings> settings, IProductService productService)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _orderCollection = database.GetCollection<OrderModel>("Orders");
            _productService = productService; // Assigning ProductService
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

            var orders = _orderCollection.Find(filter).ToList();

            // Filter out only the order items for the specific vendor
            var filteredOrders = orders.Select(order => new OrderModel
            {
                Id = order.Id,
                PlacedDate = order.PlacedDate,
                CustomerId = order.CustomerId,
                Total = order.Total,
                DeliveryAddress = order.DeliveryAddress,
                DeliveryStatus = order.DeliveryStatus,
                DeliveryDate = order.DeliveryDate,
                CancelDetails = order.CancelDetails,
                Items = order.Items
                    .Where(item => item.VenderId == vendorId)
                    .Select(item => new VendorOrderItems
                    {
                        VenderId = item.VenderId,
                        IsAccepted = item.IsAccepted,
                        OrderItems = item.OrderItems.Select(oi =>
                        {
                            // Fetch product details
                            var product = _productService.GetProductById(oi.ProductId);
                            return new OrderItemModel
                            {
                                ProductId = oi.ProductId,
                                Quantity = oi.Quantity,
                                ProductDetails = product // Enrich with product details
                            };
                        }).ToList()
                    }).ToList()
            }).ToList();

            return filteredOrders;
        }

        public void UpdateOrder(string id, OrderModel orderModel)
        {
            var filter = Builders<OrderModel>.Filter.Eq(o => o.Id, id);
            _orderCollection.ReplaceOne(filter, orderModel);
        }
    }
}
