using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EADBackend.Models
{
    public class VendorModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public required string Username { get; set; }
        public string Password { get; set; } = "";
        public bool IsVerified { get; set; } = false;
        public bool IsDeactivated { get; set; } = false;
    }
}