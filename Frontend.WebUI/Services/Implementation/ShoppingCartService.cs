using Frontend.WebUI.Models.DTOs;
using Frontend.WebUI.Services.Interface;
using Frontend.WebUI.Utility;

namespace Frontend.WebUI.Services.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IBaseService _baseService;

        public ShoppingCartService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> ApplyCoupon(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = Constants.ApiType.POST,
                Url = Constants.ShoppingCartAPIBase + "/api/ShoppingCartAPI/ApplyCoupon",
                Data = cartDTO
            });
        }

        public async Task<ResponseDTO?> GetCartByUserId(string userId)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = Utility.Constants.ApiType.GET,
                Url = Constants.ShoppingCartAPIBase + "/api/ShoppingCartAPI/GetCart/" + userId,
            });
        }

        public async Task<ResponseDTO?> RemoveCart(int cartDetailId)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = Constants.ApiType.POST,
                Url = Constants.ShoppingCartAPIBase + "/api/ShoppingCartAPI/RemoveCart",
                Data= cartDetailId
            });
        }

        public async Task<ResponseDTO?> UpSertCart(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = Constants.ApiType.POST,
                Url = Constants.ShoppingCartAPIBase + "/api/ShoppingCartAPI/CartUpSert",
                Data = cartDTO
            });
        }
    }
}
