// IT21105302, Fernando U.S.L, ICartService
using EADBackend.Models;

namespace EADBackend.Services.Interfaces;

public interface ICartService
{
    IEnumerable<CartModel> GetAllCarts();
    CartModel GetCartById(string id);
    void CreateCart(CartModel cartModel);
    CartModel? GetCartByUserId(string userId);
    void UpdateCart(string id, CartModel cartModel);
    void DeleteCart(string id);
    
    void AddItemToCart(string userId, CartItemModel newItem);
    void RemoveItemFromCart(string userId, string productId);
    void UpdateItemQuantity(string userId, string productId, int newQuantity);
}
