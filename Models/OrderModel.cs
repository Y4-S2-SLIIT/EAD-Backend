using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace EADBackend.Models
{
    public class OrderModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string PlacedDate { get; set; } = DateTime.Now.ToString();
        public required string CustomerId { get; set; }
        public double Total { get; set; }
        public required string DeliveryAddress { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.Pending;
        public List<VendorOrderItems> Items { get; set; } = [];
        public string DeliveryDate { get; set; } = DateTime.Now.AddDays(7).ToString();
        public CancelDetails CancelDetails { get; set; } = new CancelDetails();
    }

    public enum DeliveryStatus
    {
        Pending,
        Shipped,
        Delivered,
        Cancelled
    }

    public enum CancelStatus
    {
        Pending,
        Canceled,
        Rejected
    }

    public class CancelDetails
    {
        public bool Requested { get; set; } = false;
        public string Details { get; set; } = string.Empty;
        public CancelStatus Status { get; set; } = CancelStatus.Pending;
    }

    public class VendorOrderItems
    {
        public string VenderId { get; set; } = string.Empty;
        public bool IsAccepted { get; set; } = false;
        public List<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();
    }

    public class OrderItemModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public required string ProductId { get; set; }
        public required int Quantity { get; set; }
    }
}