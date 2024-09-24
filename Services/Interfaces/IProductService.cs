using EADBackend.Models;

namespace EADBackend.Services.Interfaces;
public interface IProductService
{
    IEnumerable<ProductModel> GetAllProducts();
    ProductModel GetProductById(string id);
    void CreateProduct(ProductModel productModel);
    void UpdateProduct(string id, ProductModel productModel);
    void DeleteProduct(string id);
    IEnumerable<ProductModel> GetProductsByCategory(string category);
    IEnumerable<ProductModel> GetProductsByBrand(string brand);
    IEnumerable<ProductModel> GetProductsByVendor(string vendorId);
    void AddReview(string productId, ReviewModel reviewModel);
    void DeleteReview(string productId, string reviewId);
    void UpdateReview(string productId, string reviewId, ReviewModel reviewModel);
}