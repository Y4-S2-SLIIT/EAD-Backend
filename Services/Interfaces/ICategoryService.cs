using EADBackend.Models;

namespace EADBackend.Services.Interfaces;
public interface ICategoryService
{
    IEnumerable<CategoryModel> GetAllCategories();
    CategoryModel GetCategoryById(string id);
    void CreateCategory(CategoryModel categoryModel);
    void UpdateCategory(string id, CategoryModel categoryModel);
    void DeleteCategory(string id);
}