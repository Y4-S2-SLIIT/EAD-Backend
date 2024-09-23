using EADBackend.Models;

namespace EADBackend.Services.Interfaces;

public interface ICartService
{
    IEnumerable<CartModel> GetAllCarts();
    CartModel GetCartById(string id);
    void CreateCart(CartModel cartModel);
    void UpdateCart(string id, CartModel cartModel);
    void DeleteCart(string id);
}