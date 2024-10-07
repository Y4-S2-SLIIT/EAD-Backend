// IT21105302, Fernando U.S.L, CartService
using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EADBackend.Services
{
    public class CartService : ICartService
    {
        private readonly IMongoCollection<CartModel> _cartCollection;
        private readonly IProductService _productService;

        public CartService(IMongoClient client, IOptions<MongoDbSettings> settings, IProductService productService)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _cartCollection = database.GetCollection<CartModel>("Carts");
            _productService = productService;
        }

        // Creates a new cart document if a cart for the same user doesn't already exist
        public void CreateCart(CartModel cartModel)
        {
            // Check if a cart already exists for the given UserId
            var existingCart = _cartCollection.Find(c => c.UserId == cartModel.UserId).FirstOrDefault();
            if (existingCart != null)
            {
                // Optionally, you could throw an exception or return a message indicating the user already has a cart
                throw new InvalidOperationException("A cart already exists for this user.");
            }
            // If no cart exists, insert a new one
            _cartCollection.InsertOne(cartModel);
        }

        // Get cart by userId and populate cart items with product details
        public CartModel? GetCartByUserId(string userId)
        {
            var cart = _cartCollection.Find(c => c.UserId == userId).FirstOrDefault();

            if (cart == null) return null;

            // Populate each cart item with product details
            foreach (var item in cart.Items)
            {
                var product = _productService.GetProductById(item.ProductId);
                if (product != null)
                {
                    item.ProductDetails = product; // Populate ProductDetails
                }
            }

            return cart;
        }

        public void AddItemToCart(string userId, CartItemModel newItem)
        {
            var existingCart = _cartCollection.Find(c => c.UserId == userId).FirstOrDefault();

            if (existingCart != null)
            {
                // Check if the item already exists in the cart
                var existingItem = existingCart.Items.Find(i => i.ProductId == newItem.ProductId);

                if (existingItem != null)
                {
                    // Update quantity if item already exists
                    existingItem.Quantity += newItem.Quantity;
                }
                else
                {
                    // Add new item to the cart
                    existingCart.Items.Add(newItem);
                }

                // Update the cart in the database
                _cartCollection.ReplaceOne(c => c.Id == existingCart.Id, existingCart);
            }
            else
            {
                // Create a new cart and add the item
                var newCart = new CartModel
                {
                    UserId = userId,
                    Items = new List<CartItemModel> { newItem }
                };
                _cartCollection.InsertOne(newCart);
            }
        }

        public void RemoveItemFromCart(string userId, string productId)
        {
            var existingCart = _cartCollection.Find(c => c.UserId == userId).FirstOrDefault();

            if (existingCart != null)
            {
                // Remove the item from the cart
                existingCart.Items.RemoveAll(i => i.ProductId == productId);

                // Update the cart in the database
                _cartCollection.ReplaceOne(c => c.Id == existingCart.Id, existingCart);
            }
        }

        public void UpdateItemQuantity(string userId, string productId, int newQuantity)
        {
            var existingCart = _cartCollection.Find(c => c.UserId == userId).FirstOrDefault();

            if (existingCart != null)
            {
                var existingItem = existingCart.Items.Find(i => i.ProductId == productId);

                if (existingItem != null)
                {
                    // Update the quantity of the existing item
                    existingItem.Quantity = newQuantity;

                    // Update the cart in the database
                    _cartCollection.ReplaceOne(c => c.Id == existingCart.Id, existingCart);
                }
                else
                {
                    throw new KeyNotFoundException("Item not found in cart.");
                }
            }
            else
            {
                throw new InvalidOperationException("No cart found for the user.");
            }
        }

        // Deletes a cart document from the Carts collection by its ID
        public void DeleteCart(string id)
        {
            var filter = Builders<CartModel>.Filter.Eq(p => p.Id, id);
            _cartCollection.DeleteOne(filter);
        }

        // Retrieves all cart documents from the Carts collection
        public IEnumerable<CartModel> GetAllCarts()
        {
            return _cartCollection.Find(_ => true).ToList();
        }

        // Retrieves a single cart document by its ID
        public CartModel GetCartById(string id)
        {
            return _cartCollection.Find(p => p.Id == id).FirstOrDefault();
        }

        // Updates a cart document by its ID with the provided cart details
        public void UpdateCart(string id, CartModel cartModel)
        {
            var filter = Builders<CartModel>.Filter.Eq(p => p.Id, id);
            _cartCollection.ReplaceOne(filter, cartModel);
        }
    }
}
