using EADBackend.Models;

namespace EADBackend.Services.Interfaces;

public interface IVendorService
{
    string? ValidateVendor(string username, string password);
    bool IsEmailTaken(string email);
    bool IsUsernameTaken(string username);
    void CreateVendor(VendorModel vendorModel);
    VendorModel GetVendorById(string id);
    IEnumerable<VendorModel> GetAllVendors();
    void UpdateVendor(string id, VendorModel vendorModel);
    void DeleteVendor(string id);
}