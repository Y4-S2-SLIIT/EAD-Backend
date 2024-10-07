using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EADBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewsService _reviewService;

        private readonly string notFoundMessage = "Review not found";

        public ReviewController(IReviewsService reviewService)
        {
            _reviewService = reviewService;
        }

        // Get review by ID
        [HttpGet("/{reviewId}")]
        [ProducesResponseType(typeof(ReviewsModel), 200)]
        public IActionResult GetReviewById( string reviewId)
        {
            var review = _reviewService.GetReviewById(reviewId);
            if (review == null)
                return NotFound(notFoundMessage);
            return Ok(review);
        }

        // Create review
        [HttpPost("{productId}")]
        [Authorize]
        [ProducesResponseType(typeof(ReviewsModel), 200)]
        public IActionResult CreateReview(ReviewsModel reviewModel)
        {
            _reviewService.CreateReview(reviewModel);
            return Ok(reviewModel);
        }

        // Update review
        [HttpPut("{productId}/{reviewId}")]
        [Authorize]
        [ProducesResponseType(typeof(ReviewsModel), 200)]
        public IActionResult UpdateReview(string reviewId, ReviewsModel reviewModel)
        {
            _reviewService.UpdateReview(reviewId, reviewModel);
            return Ok(reviewModel);
        }

        // Delete review
        [HttpDelete("{productId}/{reviewId}")]
        [Authorize]
        [ProducesResponseType(200)]
        public IActionResult DeleteReview(string reviewId)
        {
            _reviewService.DeleteReview(reviewId);
            return Ok();
        }

        // Get rating by vendor ID
        [HttpGet("vendor/{vendorId}")]
        [ProducesResponseType(typeof(RatingModel), 200)]
        public IActionResult GetRatingByVendorId(string vendorId)
        {
            var rating = _reviewService.GetRatingByVendorId(vendorId);
            return Ok(rating);
        }

        // Get reviews by vendor ID
        [HttpGet("vendor/{vendorId}/reviews")]
        [ProducesResponseType(typeof(IEnumerable<ReviewsModel>), 200)]
        public IActionResult GetReviewsByVendorId(string vendorId)
        {
            var reviews = _reviewService.GetReviewsByVendorId(vendorId);
            return Ok(reviews);
        }
    }
}