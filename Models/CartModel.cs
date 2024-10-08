// IT21105302, Fernando U.S.L, CartModel
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace EADBackend.Models
{
    public class CartModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public required string UserId { get; set; }
        public List<CartItemModel> Items { get; set; } = [];
    }

    public class CartItemModel
    {
        public required string ProductId { get; set; }
        public required int Quantity { get; set; }
        public ProductModel? ProductDetails { get; set; }
    }

    public class UpdateQuantityModel
    {
        public int Quantity { get; set; }
    }
}