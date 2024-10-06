using EADBackend.Models;

namespace EADBackend.Services.Interfaces;
public interface IOrderService
{
    OrderModel CreateOrder(OrderModel orderModel);
    OrderModel GetOrderById(string id);
    IEnumerable<OrderModel> GetOrdersByCustomerId(string customerId);
    IEnumerable<OrderModel> GetAllOrders();
    void UpdateOrder(string id, OrderModel orderModel);
    void DeleteOrder(string id);
    IEnumerable<OrderModel> GetOrdersByVendorId(string vendorId);
    void UpdateOrderStatus(string id, string status);
}