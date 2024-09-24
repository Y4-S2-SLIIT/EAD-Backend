using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EADBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        private readonly string notFoundMessage = "Product not found";

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // Get all products
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetAllProducts()
        {
            var products = _productService.GetAllProducts();
            return Ok(products);
        }

        // Get product by ID
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetProductById(string id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound(notFoundMessage);
            return Ok(product);
        }

        // Get products by brand
        [HttpGet("brand/{brand}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetProductsByBrand(string brand)
        {
            var products = _productService.GetProductsByBrand(brand);
            return Ok(products);
        }

        // Get products by category
        [HttpGet("category/{category}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetProductsByCategory(string category)
        {
            var products = _productService.GetProductsByCategory(category);
            return Ok(products);
        }

        // Get products by vendor
        [HttpGet("vendor/{vendorId}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetProductsByVendor(string vendorId)
        {
            var products = _productService.GetProductsByVendor(vendorId);
            return Ok(products);
        }

        // Create a new product
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult CreateProduct([FromBody] ProductModel productModel)
        {
            _productService.CreateProduct(productModel);
            return Ok("Product created successfully");
        }

        // Update product by ID
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult UpdateProduct(string id, [FromBody] ProductModel productModel)
        {
            var existingProduct = _productService.GetProductById(id);
            if (existingProduct == null)
                return NotFound(notFoundMessage);

            _productService.UpdateProduct(id, productModel);
            return Ok("Product updated successfully");
        }

        // Delete a product by ID
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult DeleteProduct(string id)
        {
            var existingProduct = _productService.GetProductById(id);
            if (existingProduct == null)
                return NotFound(notFoundMessage);

            _productService.DeleteProduct(id);
            return Ok("Product deleted successfully");
        }

        // Add a review to a product
        [HttpPost("{productId}/reviews")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult AddReview(string productId, [FromBody] ReviewModel reviewModel)
        {
            var existingProduct = _productService.GetProductById(productId);
            if (existingProduct == null)
                return NotFound(notFoundMessage);

            _productService.AddReview(productId, reviewModel);
            return Ok("Review added successfully");
        }

        // Update a review for a product
        [HttpPut("{productId}/reviews/{reviewId}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult UpdateReview(string productId, string reviewId, [FromBody] ReviewModel reviewModel)
        {
            var existingProduct = _productService.GetProductById(productId);
            if (existingProduct == null)
                return NotFound(notFoundMessage);

            _productService.UpdateReview(productId, reviewId, reviewModel);
            return Ok("Review updated successfully");
        }

        // Delete a review from a product
        [HttpDelete("{productId}/reviews/{reviewId}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult DeleteReview(string productId, string reviewId)
        {
            var existingProduct = _productService.GetProductById(productId);
            if (existingProduct == null)
                return NotFound(notFoundMessage);

            _productService.DeleteReview(productId, reviewId);
            return Ok("Review deleted successfully");
        }
    }
}