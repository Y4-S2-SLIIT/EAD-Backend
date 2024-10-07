using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace EADBackend.Models
{
    public class ReviewsModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public required string CusID { get; set; }
        public required string VendorId { get; set; }
        public required string OrderId { get; set; }
        public required int Rating { get; set; }
        public required string Description { get; set; }
    }
}