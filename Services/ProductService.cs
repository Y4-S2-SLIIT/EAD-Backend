// IT21105302, Fernando U.S.L, ProductService
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

        // Creates a new product document in the Products collection
        public void CreateProduct(ProductModel productModel)
        {
            _productCollection.InsertOne(productModel);
        }

        // Deletes a product document from the Products collection by its ID
        public void DeleteProduct(string id)
        {
            var filter = Builders<ProductModel>.Filter.Eq(p => p.Id, id);
            _productCollection.DeleteOne(filter);
        }

        // Retrieves all product documents from the Products collection
        public IEnumerable<ProductModel> GetAllProducts()
        {
            return _productCollection.Find(_ => true).ToList();
        }

        // Retrieves a single product document by its ID
        public ProductModel GetProductById(string id)
        {
            return _productCollection.Find(p => p.Id == id).FirstOrDefault();
        }

        // Retrieves all product documents that match the specified brand
        public IEnumerable<ProductModel> GetProductsByBrand(string brand)
        {
            return _productCollection.Find(p => p.Brand == brand).ToList();
        }

        // Retrieves all product documents that belong to a specific category by its ID
        public IEnumerable<ProductModel> GetProductsByCategory(string categoryId)
        {
            return _productCollection.Find(p => p.Category.Id == categoryId).ToList();
        }

        // Retrieves all product documents that belong to a specific vendor by its vendor ID
        public IEnumerable<ProductModel> GetProductsByVendor(string vendorId)
        {
            return _productCollection.Find(p => p.VendorId == vendorId).ToList();
        }

        // Updates a product document by its ID with the provided product details
        public void UpdateProduct(string id, ProductModel productModel)
        {
            var filter = Builders<ProductModel>.Filter.Eq(p => p.Id, id);
            _productCollection.ReplaceOne(filter, productModel);
        }

        // Adds a new review to a specific product and recalculates the product's average rating
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

        // Updates an existing review for a specific product and recalculates the product's average rating
        public void UpdateReview(string productId, string reviewId, ReviewModel reviewModel)
        {
            // Find the product by its ID and the specific review by the review ID
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

        // Deletes a review from a specific product and recalculates the product's average rating
        public void DeleteReview(string productId, string reviewId)
        {
            // Filter by product ID and remove the review with the specified review ID
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