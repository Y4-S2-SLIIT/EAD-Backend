namespace EADBackend.Models
{
    public class RatingModel
    {
        public string? VendorId { get; set; } // Make it nullable
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}