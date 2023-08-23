using Frontend.WebUI.Models.DTOs;

namespace Frontend.WebUI.Services.Interface
{
    public interface IShoppingCartService
    {
        Task<ResponseDTO?> GetCartByUserId(string userId);
        Task<ResponseDTO?> UpSertCart(CartDTO cartDTO);
        Task<ResponseDTO?> RemoveCart(int cartDetailId);
        Task<ResponseDTO?> ApplyCoupon(CartDTO cartDTO);
    }
}
