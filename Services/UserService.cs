using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EADBackend.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<UserModel> _users;

        public UserService(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _users = database.GetCollection<UserModel>("Users");

            CreateIndexes();
        }

        private void CreateIndexes()
        {
            // Create unique index for email
            var emailIndexKeysDefinition = Builders<UserModel>.IndexKeys.Ascending(u => u.Email);
            var emailIndexOptions = new CreateIndexOptions { Unique = true };
            var emailIndexModel = new CreateIndexModel<UserModel>(emailIndexKeysDefinition, emailIndexOptions);
            _users.Indexes.CreateOne(emailIndexModel);

            // Create unique index for username
            var usernameIndexKeysDefinition = Builders<UserModel>.IndexKeys.Ascending(u => u.Username);
            var usernameIndexOptions = new CreateIndexOptions { Unique = true };
            var usernameIndexModel = new CreateIndexModel<UserModel>(usernameIndexKeysDefinition, usernameIndexOptions);
            _users.Indexes.CreateOne(usernameIndexModel);
        }

        public string? ValidateUser(string username, string password)
        {
            var user = _users.Find(u => u.Username == username).FirstOrDefault();

            // Return false if user doesn't exist or password doesn't match
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return user.Id;
        }

        public void CreateUser(UserModel userModel)
        {
            // Hash the password before saving the user
            userModel.Password = BCrypt.Net.BCrypt.HashPassword(userModel.Password);
            _users.InsertOne(userModel);
        }

        public UserModel GetUserById(string id)
        {
            return _users.Find(u => u.Id == id).FirstOrDefault();
        }

        public IEnumerable<UserModel> GetAllUsers()
        {
            return _users.Find(_ => true).ToList();
        }

        public void UpdateUser(string id, UserModel userModel)
        {
            // Check if the user exists
            var existingUser = _users.Find(u => u.Id == id).FirstOrDefault();
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Log existing user and user model for debugging
            // Console.WriteLine($"Existing User: {JsonConvert.SerializeObject(existingUser)}");
            // Console.WriteLine($"Update Model: {JsonConvert.SerializeObject(userModel)}");

            // Create a list to hold update definitions
            var updateDefinitions = new List<UpdateDefinition<UserModel>>();

            // If the password has been updated, hash the new password
            if (!string.IsNullOrEmpty(userModel.Password))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userModel.Password);
                updateDefinitions.Add(Builders<UserModel>.Update.Set(u => u.Password, hashedPassword));
            }

            // Update other fields
            if (!string.IsNullOrEmpty(userModel.Email) && userModel.Email != existingUser.Email)
            {
                updateDefinitions.Add(Builders<UserModel>.Update.Set(u => u.Email, userModel.Email));
            }

            if (!string.IsNullOrEmpty(userModel.Username) && userModel.Username != existingUser.Username)
            {
                updateDefinitions.Add(Builders<UserModel>.Update.Set(u => u.Username, userModel.Username));
            }

            // update FisrtName and LastName
            if (!string.IsNullOrEmpty(userModel.FirstName))
            {
                updateDefinitions.Add(Builders<UserModel>.Update.Set(u => u.FirstName, userModel.FirstName));
            }

            if (!string.IsNullOrEmpty(userModel.LastName))
            {
                updateDefinitions.Add(Builders<UserModel>.Update.Set(u => u.LastName, userModel.LastName));
            }

            // update Phone
            if (!string.IsNullOrEmpty(userModel.Phone))
            {
                updateDefinitions.Add(Builders<UserModel>.Update.Set(u => u.Phone, userModel.Phone));
            }

            // update UserType
            if (!string.IsNullOrEmpty(userModel.UserType))
            {
                updateDefinitions.Add(Builders<UserModel>.Update.Set(u => u.UserType, userModel.UserType));
            }

            // If no fields are updated, skip the update operation
            if (updateDefinitions.Count == 0)
            {
                Console.WriteLine("No fields to update.");
                return; // Or throw an exception if you want to indicate no changes were made
            }

            // Combine all update definitions into a single update operation
            var update = Builders<UserModel>.Update.Combine(updateDefinitions);

            try
            {
                // Perform the update
                var result = _users.UpdateOne(u => u.Id == id, update);

                // Log result for debugging
                // Console.WriteLine($"MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}");

                // Check if the update operation matched any documents
                if (result.MatchedCount == 0)
                {
                    throw new InvalidOperationException("Update failed. User not found.");
                }

                if (result.ModifiedCount == 0)
                {
                    Console.WriteLine("No fields were updated.");
                }
            }
            catch (Exception ex)
            {
                // Handle potential exceptions
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw; // Re-throw the exception to be handled by the caller if necessary
            }
        }

        public void DeleteUser(string id)
        {
            try
            {
                var result = _users.DeleteOne(u => u.Id == id);
                if (result.DeletedCount == 0)
                {
                    throw new InvalidOperationException("User not found.");
                }
            }
            catch (Exception ex)
            {
                // Handle potential exceptions
                Console.WriteLine($"Error deleting user: {ex.Message}");
                throw;
            }
        }

        public bool IsEmailTaken(string email)
        {
            return _users.Find(u => u.Email == email).FirstOrDefault() != null;
        }

        public bool IsUsernameTaken(string username)
        {
            return _users.Find(u => u.Username == username).FirstOrDefault() != null;
        }
    }
}