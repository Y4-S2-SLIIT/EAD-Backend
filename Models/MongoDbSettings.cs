// Models/MongoDbSettings.cs
namespace EADBackend.Models
{
    public class MongoDbSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
    }
}