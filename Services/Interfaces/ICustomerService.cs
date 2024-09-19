using EADBackend.Models;

namespace EADBackend.Services.Interfaces;
public interface ICustomerService
{
    bool ValidateCustomer(string username, string password);
    bool IsEmailTaken(string email);
    bool IsUsernameTaken(string username);
    void CreateCustomer(CustomerModel customerModel);
    CustomerModel GetCustomerById(string id);
    IEnumerable<CustomerModel> GetAllCustomers();
    void UpdateCustomer(string id, CustomerModel customerModel);
    void DeleteCustomer(string id);
}
