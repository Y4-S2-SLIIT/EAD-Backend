using MongoDB.Driver;
using EADBackend.Models;
using EADBackend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EADBackend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<CategoryModel> _categoryCollection;

        public CategoryService(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var databaseName = settings.Value.DatabaseName;
            var database = client.GetDatabase(databaseName);
            _categoryCollection = database.GetCollection<CategoryModel>("Categories");
        }

        public void CreateCategory(CategoryModel categoryModel)
        {
            _categoryCollection.InsertOne(categoryModel);
        }

        public void DeleteCategory(string id)
        {
            var filter = Builders<CategoryModel>.Filter.Eq(p => p.Id, id);
            _categoryCollection.DeleteOne(filter);
        }

        public IEnumerable<CategoryModel> GetAllCategories()
        {
            return _categoryCollection.Find(_ => true).ToList();
        }

        public CategoryModel GetCategoryById(string id)
        {
            return _categoryCollection.Find(p => p.Id == id).FirstOrDefault();
        }

        public void UpdateCategory(string id, CategoryModel categoryModel)
        {
            var filter = Builders<CategoryModel>.Filter.Eq(p => p.Id, id);
            _categoryCollection.ReplaceOne(filter, categoryModel);
        }
    }
}