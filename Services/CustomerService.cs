using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EADBackend.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMongoCollection<CustomerModel> _customers;

        public CustomerService(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _customers = database.GetCollection<CustomerModel>("Customers");

            CreateIndexes();
        }

        private void CreateIndexes()
        {
            // Create unique index for email
            var emailIndexKeysDefinition = Builders<CustomerModel>.IndexKeys.Ascending(c => c.Email);
            var emailIndexOptions = new CreateIndexOptions { Unique = true };
            var emailIndexModel = new CreateIndexModel<CustomerModel>(emailIndexKeysDefinition, emailIndexOptions);
            _customers.Indexes.CreateOne(emailIndexModel);

            // Create unique index for username
            var usernameIndexKeysDefinition = Builders<CustomerModel>.IndexKeys.Ascending(c => c.Username);
            var usernameIndexOptions = new CreateIndexOptions { Unique = true };
            var usernameIndexModel = new CreateIndexModel<CustomerModel>(usernameIndexKeysDefinition, usernameIndexOptions);
            _customers.Indexes.CreateOne(usernameIndexModel);
        }

        public bool ValidateCustomer(string username, string password)
        {
            var customer = _customers.Find(c => c.Username == username).FirstOrDefault();

            // Return false if customer doesn't exist or password doesn't match
            if (customer == null || !BCrypt.Net.BCrypt.Verify(password, customer.Password))
            {
                return false;
            }

            return true;
        }

        public void CreateCustomer(CustomerModel customerModel)
        {
            // Hash the password before saving the customer
            customerModel.Password = BCrypt.Net.BCrypt.HashPassword(customerModel.Password);
            _customers.InsertOne(customerModel);
        }

        public CustomerModel GetCustomerById(string id)
        {
            return _customers.Find(c => c.Id == id).FirstOrDefault();
        }

        public IEnumerable<CustomerModel> GetAllCustomers()
        {
            return _customers.Find(_ => true).ToList();
        }

        public void UpdateCustomer(string id, CustomerModel customerModel)
        {
            // Check if the customer exists
            var existingCustomer = _customers.Find(c => c.Id == id).FirstOrDefault();
            if (existingCustomer == null)
            {
                throw new InvalidOperationException("Customer not found.");
            }

            // Create a list to hold update definitions
            var updateDefinitions = new List<UpdateDefinition<CustomerModel>>();

            // If the password has been updated, hash the new password
            if (!string.IsNullOrEmpty(customerModel.Password))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(customerModel.Password);
                updateDefinitions.Add(Builders<CustomerModel>.Update.Set(c => c.Password, hashedPassword));
            }

            // Update other fields
            if (!string.IsNullOrEmpty(customerModel.Email) && customerModel.Email != existingCustomer.Email)
            {
                updateDefinitions.Add(Builders<CustomerModel>.Update.Set(c => c.Email, customerModel.Email));
            }

            if (!string.IsNullOrEmpty(customerModel.Username) && customerModel.Username != existingCustomer.Username)
            {
                updateDefinitions.Add(Builders<CustomerModel>.Update.Set(c => c.Username, customerModel.Username));
            }

            // Update FirstName and LastName
            if (!string.IsNullOrEmpty(customerModel.FirstName))
            {
                updateDefinitions.Add(Builders<CustomerModel>.Update.Set(c => c.FirstName, customerModel.FirstName));
            }

            if (!string.IsNullOrEmpty(customerModel.LastName))
            {
                updateDefinitions.Add(Builders<CustomerModel>.Update.Set(c => c.LastName, customerModel.LastName));
            }

            // Update Phone
            if (!string.IsNullOrEmpty(customerModel.Phone))
            {
                updateDefinitions.Add(Builders<CustomerModel>.Update.Set(c => c.Phone, customerModel.Phone));
            }

            // Update Address
            if (!string.IsNullOrEmpty(customerModel.Address))
            {
                updateDefinitions.Add(Builders<CustomerModel>.Update.Set(c => c.Address, customerModel.Address));
            }

            // If no fields are updated, skip the update operation
            if (updateDefinitions.Count == 0)
            {
                Console.WriteLine("No fields to update.");
                return;
            }

            // Combine all update definitions into a single update operation
            var update = Builders<CustomerModel>.Update.Combine(updateDefinitions);

            try
            {
                // Perform the update
                var result = _customers.UpdateOne(c => c.Id == id, update);

                // Check if the update operation matched any documents
                if (result.MatchedCount == 0)
                {
                    throw new InvalidOperationException("Update failed. Customer not found.");
                }

                if (result.ModifiedCount == 0)
                {
                    Console.WriteLine("No fields were updated.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
                throw;
            }
        }

        public void DeleteCustomer(string id)
        {
            try
            {
                var result = _customers.DeleteOne(c => c.Id == id);
                if (result.DeletedCount == 0)
                {
                    throw new InvalidOperationException("Customer not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
                throw;
            }
        }

        public bool IsEmailTaken(string email)
        {
            return _customers.Find(c => c.Email == email).FirstOrDefault() != null;
        }

        public bool IsUsernameTaken(string username)
        {
            return _customers.Find(c => c.Username == username).FirstOrDefault() != null;
        }

        public void VerifyCustomer(string id)
        {
            var update = Builders<CustomerModel>.Update.Set(c => c.IsVerified, true);
            var result = _customers.UpdateOne(c => c.Id == id, update);

            if (result.MatchedCount == 0)
            {
                throw new InvalidOperationException("Customer not found.");
            }

            if (result.ModifiedCount == 0)
            {
                Console.WriteLine("No fields were updated.");
                return;
            }

            Console.WriteLine("Customer verified successfully.");
        }
    }
}