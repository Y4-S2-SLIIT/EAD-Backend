using Microsoft.AspNetCore.Mvc;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EADBackend.Services;

namespace EADBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryModel>), 200)]
        public IActionResult GetAllCategories()
        {
            return Ok(_categoryService.GetAllCategories());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryModel), 200)]
        public IActionResult GetCategoryById(string id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound(new { status = 404, error = "Category not found." });
            }

            return Ok(category);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult CreateCategory([FromBody] CategoryModel categoryModel)
        {
            try
            {
                _categoryService.CreateCategory(categoryModel);
                return Ok(new { status = 200, added = new { Message = "Category created successfully." } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 400, error = "An unexpected error occurred.", exception = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult UpdateCategory(string id, [FromBody] CategoryModel categoryModel)
        {
            try
            {
                _categoryService.UpdateCategory(id, categoryModel);
                return Ok(new { status = 200, added = new { Message = "Category updated successfully." } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 400, error = "An unexpected error occurred.", exception = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult DeleteCategory(string id)
        {
            try
            {
                _categoryService.DeleteCategory(id);
                return Ok(new { status = 200, added = new { Message = "Category deleted successfully." } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 400, error = "An unexpected error occurred.", exception = ex.Message });
            }
        }
    }
}