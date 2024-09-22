using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace EADBackend.Models
{
    public class ProductModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public required string Name { get; set; }
        public required string Brand { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required float Price { get; set; }
        public required string Image { get; set; }
        public required string Stock { get; set; }
        public required string VendorId { get; set; }
        public List<ReviewModel> Reviews { get; set; } = [];
        public float Rating { get; set; } = 0;
    }

    // Review model
    public class ReviewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public required string CusID { get; set; }
        public required string Description { get; set; }
        public required int Rating { get; set; }
    }
}