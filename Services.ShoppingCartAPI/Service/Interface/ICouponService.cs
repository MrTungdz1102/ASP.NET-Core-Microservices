using Services.ShoppingCartAPI.Models.DTOs;

namespace Services.ShoppingCartAPI.Service.Interface
{
    public interface ICouponService
    {
        Task<CouponDTO> GetCoupon(string couponCode);
    }
}
