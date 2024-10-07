using EADBackend.Models;

namespace EADBackend.Services.Interfaces
{
    public interface IReviewsService
    {
        ReviewsModel GetReviewById(string reviewId);
        void CreateReview(ReviewsModel reviewModel);
        void UpdateReview(string reviewId, ReviewsModel reviewModel);
        void DeleteReview(string reviewId);

        RatingModel GetRatingByVendorId(string vendorId);
        IEnumerable<ReviewsModel> GetReviewsByVendorId(string vendorId); 

    }
}