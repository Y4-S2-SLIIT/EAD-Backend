using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EADBackend.Services
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<ProductModel> _productCollection;

        public ProductService(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _productCollection = database.GetCollection<ProductModel>("Products");
        }

        public void CreateProduct(ProductModel productModel)
        {
            _productCollection.InsertOne(productModel);
        }

        public void DeleteProduct(string id)
        {
            var filter = Builders<ProductModel>.Filter.Eq(p => p.Id, id);
            _productCollection.DeleteOne(filter);
        }

        public IEnumerable<ProductModel> GetAllProducts()
        {
            return _productCollection.Find(_ => true).ToList();
        }

        public ProductModel GetProductById(string id)
        {
            return _productCollection.Find(p => p.Id == id).FirstOrDefault();
        }

        public IEnumerable<ProductModel> GetProductsByBrand(string brand)
        {
            return _productCollection.Find(p => p.Brand == brand).ToList();
        }

        public IEnumerable<ProductModel> GetProductsByCategory(string category)
        {
            return _productCollection.Find(p => p.Category == category).ToList();
        }

        public IEnumerable<ProductModel> GetProductsByVendor(string vendorId)
        {
            return _productCollection.Find(p => p.VendorId == vendorId).ToList();
        }

        public void UpdateProduct(string id, ProductModel productModel)
        {
            var filter = Builders<ProductModel>.Filter.Eq(p => p.Id, id);
            _productCollection.ReplaceOne(filter, productModel);
        }

        public void AddReview(string productId, ReviewModel reviewModel)
        {
            var filter = Builders<ProductModel>.Filter.Eq(p => p.Id, productId);
            var update = Builders<ProductModel>.Update.Push(p => p.Reviews, reviewModel);

            // Add the review
            _productCollection.UpdateOne(filter, update);

            // Recalculate the product's average rating
            var product = _productCollection.Find(filter).FirstOrDefault();
            if (product != null)
            {
                var newRating = product.Reviews.Average(r => r.Rating);
                var ratingUpdate = Builders<ProductModel>.Update.Set(p => p.Rating, newRating);
                _productCollection.UpdateOne(filter, ratingUpdate);
            }
        }

        public void UpdateReview(string productId, string reviewId, ReviewModel reviewModel)
        {
            // Find the product by its ID and the specific review by the customer's ID (CusID)
            var filter = Builders<ProductModel>.Filter.And(
                Builders<ProductModel>.Filter.Eq(p => p.Id, productId),
                Builders<ProductModel>.Filter.ElemMatch(p => p.Reviews, r => r.Id == reviewId)
            );

            // Update the review
            var update = Builders<ProductModel>.Update.Set(p => p.Reviews[-1], reviewModel);
            _productCollection.UpdateOne(filter, update);

            // Recalculate the product's average rating after the review update
            var product = _productCollection.Find(Builders<ProductModel>.Filter.Eq(p => p.Id, productId)).FirstOrDefault();
            if (product != null)
            {
                var newRating = product.Reviews.Average(r => r.Rating);
                var ratingUpdate = Builders<ProductModel>.Update.Set(p => p.Rating, newRating);
                _productCollection.UpdateOne(Builders<ProductModel>.Filter.Eq(p => p.Id, productId), ratingUpdate);
            }
        }

        public void DeleteReview(string productId, string reviewId)
        {
            // Filter by product ID and remove the review with the specified reviewId
            var filter = Builders<ProductModel>.Filter.Eq(p => p.Id, productId);
            var update = Builders<ProductModel>.Update.PullFilter(p => p.Reviews, r => r.Id == reviewId);
            _productCollection.UpdateOne(filter, update);

            // Recalculate the product's average rating after the review deletion
            var product = _productCollection.Find(filter).FirstOrDefault();
            if (product != null && product.Reviews.Any())
            {
                var newRating = product.Reviews.Average(r => r.Rating);
                var ratingUpdate = Builders<ProductModel>.Update.Set(p => p.Rating, newRating);
                _productCollection.UpdateOne(filter, ratingUpdate);
            }
            else
            {
                // If there are no reviews left, reset the rating to 0
                var ratingReset = Builders<ProductModel>.Update.Set(p => p.Rating, 0);
                _productCollection.UpdateOne(filter, ratingReset);
            }
        }
    }
}