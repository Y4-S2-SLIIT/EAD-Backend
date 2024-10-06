using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Linq;

namespace EADBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<OrderModel> _orderCollection;
        private readonly IProductService _productService;

        public OrderService(IMongoClient client, IOptions<MongoDbSettings> settings, IProductService productService)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _orderCollection = database.GetCollection<OrderModel>("Orders");
            _productService = productService;
        }

        public OrderModel CreateOrder(OrderModel orderModel)
        {
            // Insert the order into the collection
            _orderCollection.InsertOne(orderModel);

            // Loop through each item in the order to update stock
            foreach (var vendorItem in orderModel.Items)
            {
                foreach (var orderItem in vendorItem.OrderItems)
                {
                    // Get the product by ID
                    var product = _productService.GetProductById(orderItem.ProductId);

                    // Check if the product exists and has enough stock
                    if (product != null && product.Stock >= orderItem.Quantity)
                    {
                        // Reduce the product's stock by the order quantity
                        product.Stock -= orderItem.Quantity;

                        // Update the product in the database
                        _productService.UpdateProduct(product.Id, product);
                    }
                    else
                    {
                        throw new InvalidOperationException("Insufficient stock for product: " + product.Name);
                    }
                }
            }

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

        public void UpdateOrderStatus(string id, string status)
        {
            // Validate the order ID
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Order ID cannot be null or empty.", nameof(id));
            }

            // Validate the status (Optional: You can define a list of valid statuses)
            var validStatuses = new[] { "Pending", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                throw new ArgumentException($"Invalid delivery status: {status}. Valid statuses are: {string.Join(", ", validStatuses)}", nameof(status));
            }

            var filter = Builders<OrderModel>.Filter.Eq(o => o.Id, id);
            
            // Check if the order exists
            var existingOrder = _orderCollection.Find(filter).FirstOrDefault();
            if (existingOrder == null)
            {
                throw new InvalidOperationException($"Order with ID '{id}' does not exist.");
            }

            var update = Builders<OrderModel>.Update.Set(o => o.DeliveryStatus, status);
            _orderCollection.UpdateOne(filter, update);
        }
    }
}