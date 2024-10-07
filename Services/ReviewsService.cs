using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EADBackend.Services
{
    public class ReviewsService: IReviewsService
    {
        private readonly IMongoCollection<ReviewsModel> _reviewCollection;
        private readonly IMongoCollection<RatingModel> _ratingCollection;

        public ReviewsService(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _reviewCollection = database.GetCollection<ReviewsModel>("Reviews");
            _ratingCollection = database.GetCollection<RatingModel>("Ratings");
        }

        public void CreateReview(ReviewsModel reviewModel)
        {
            _reviewCollection.InsertOne(reviewModel);
        }

        public void DeleteReview(string reviewId)
        {
            var filter = Builders<ReviewsModel>.Filter.Eq(r => r.Id, reviewId);
            _reviewCollection.DeleteOne(filter);
        }

        public void UpdateReview(string reviewId, ReviewsModel reviewModel)
        {
            var filter = Builders<ReviewsModel>.Filter.Eq(r => r.Id, reviewId);
            _reviewCollection.ReplaceOne(filter, reviewModel);
        }

        public ReviewsModel GetReviewById(string reviewId)
        {
            return _reviewCollection.Find(r => r.Id == reviewId).FirstOrDefault();
        }

        public IEnumerable<ReviewsModel> GetAllReviews()
        {
            return _reviewCollection.Find(r => true).ToList();
        }

        public IEnumerable<ReviewsModel> GetReviewsByVendorId(string vendorId)
        {
            return _reviewCollection.Find(r => r.VendorId == vendorId).ToList();
        }

        public RatingModel GetRatingByVendorId(string vendorId)
        {
            // Fetch all reviews for the vendor
            var reviews = _reviewCollection.Find(r => r.VendorId == vendorId).ToList();

            if (reviews.Count == 0)
            {
                // Return a default RatingModel if there are no reviews
                return new RatingModel
                {
                    VendorId = vendorId,
                    AverageRating = 0,
                    TotalReviews = 0
                };
            }

            // Calculate average rating
            var totalReviews = reviews.Count;
            var averageRating = reviews.Average(r => r.Rating);

            // Return the rating model
            return new RatingModel
            {
                VendorId = vendorId,
                AverageRating = averageRating,
                TotalReviews = totalReviews
            };
        }
    }
}