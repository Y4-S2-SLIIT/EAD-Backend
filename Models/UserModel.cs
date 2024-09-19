using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EADBackend.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string UserType { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}