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
        public List<VendorOrderItems> Items { get; set; } = new List<VendorOrderItems>();
    }

    public class VendorOrderItems
    {
        public string VenderId { get; set; } = string.Empty;
        public bool IsAccepted { get; set; } = false;
        public List<OrderItemModel> OrderItems { get; set; } = [];
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