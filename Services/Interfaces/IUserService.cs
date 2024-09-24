using EADBackend.Models;

namespace EADBackend.Services.Interfaces;
public interface IUserService
{
    string? ValidateUser(string username, string password);
    bool IsEmailTaken(string email);
    bool IsUsernameTaken(string username);
    void CreateUser(UserModel userModel);
    UserModel GetUserById(string id);
    IEnumerable<UserModel> GetAllUsers();
    void UpdateUser(string id, UserModel userModel);
    void DeleteUser(string id);
}