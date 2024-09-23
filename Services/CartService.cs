using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EADBackend.Services
{
    public class CartService : ICartService
    {
        private readonly IMongoCollection<CartModel> _cartCollection;

        public CartService(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _cartCollection = database.GetCollection<CartModel>("Carts");
        }

        public void CreateCart(CartModel cartModel)
        {
            _cartCollection.InsertOne(cartModel);
        }

        public void DeleteCart(string id)
        {
            var filter = Builders<CartModel>.Filter.Eq(p => p.Id, id);
            _cartCollection.DeleteOne(filter);
        }

        public IEnumerable<CartModel> GetAllCarts()
        {
            return _cartCollection.Find(_ => true).ToList();
        }

        public CartModel GetCartById(string id)
        {
            return _cartCollection.Find(p => p.Id == id).FirstOrDefault();
        }

        public void UpdateCart(string id, CartModel cartModel)
        {
            var filter = Builders<CartModel>.Filter.Eq(p => p.Id, id);
            _cartCollection.ReplaceOne(filter, cartModel);
        }
    }
}