using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EADBackend.Services
{
    public class VendorService : IVendorService
    {
        private readonly IMongoCollection<VendorModel> _vendors;

        public VendorService(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _vendors = database.GetCollection<VendorModel>("Vendors");

            CreateIndexes();
        }

        private void CreateIndexes()
        {
            // Create unique index for email
            var emailIndexKeysDefinition = Builders<VendorModel>.IndexKeys.Ascending(v => v.Email);
            var emailIndexOptions = new CreateIndexOptions { Unique = true };
            var emailIndexModel = new CreateIndexModel<VendorModel>(emailIndexKeysDefinition, emailIndexOptions);
            _vendors.Indexes.CreateOne(emailIndexModel);

            // Create unique index for username
            var usernameIndexKeysDefinition = Builders<VendorModel>.IndexKeys.Ascending(v => v.Username);
            var usernameIndexOptions = new CreateIndexOptions { Unique = true };
            var usernameIndexModel = new CreateIndexModel<VendorModel>(usernameIndexKeysDefinition, usernameIndexOptions);
            _vendors.Indexes.CreateOne(usernameIndexModel);
        }

        public bool ValidateVendor(string username, string password)
        {
            var vendor = _vendors.Find(v => v.Username == username).FirstOrDefault();

            // Return false if vendor doesn't exist or password doesn't match
            if (vendor == null || !BCrypt.Net.BCrypt.Verify(password, vendor.Password))
            {
                return false;
            }

            return true;
        }

        public void CreateVendor(VendorModel vendorModel)
        {
            // Hash the password before saving the vendor
            vendorModel.Password = BCrypt.Net.BCrypt.HashPassword(vendorModel.Password);
            _vendors.InsertOne(vendorModel);
        }

        public VendorModel GetVendorById(string id)
        {
            return _vendors.Find(v => v.Id == id).FirstOrDefault();
        }

        public IEnumerable<VendorModel> GetAllVendors()
        {
            return _vendors.Find(v => true).ToList();
        }

        public void UpdateVendor(string id, VendorModel vendorModel)
        {
            _vendors.ReplaceOne(v => v.Id == id, vendorModel);
        }

        public void DeleteVendor(string id)
        {
            _vendors.DeleteOne(v => v.Id == id);
        }

        public bool IsEmailTaken(string email)
        {
            return _vendors.Find(v => v.Email == email).Any();
        }

        public bool IsUsernameTaken(string username)
        {
            return _vendors.Find(v => v.Username == username).Any();
        }
    }
}
