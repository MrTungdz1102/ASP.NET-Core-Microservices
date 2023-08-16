using Frontend.WebUI.Models.DTOs;

namespace Frontend.WebUI.Services.Interface
{
	public interface ICouponService
	{
		Task<ResponseDTO?> GetCouponAsync(string code);
		Task<ResponseDTO?> GetAllCouponAsync();
		Task<ResponseDTO?> GetCouponByIdAsync(int id);
		Task<ResponseDTO?> CreateCouponAsync(CouponDTO couponDTO);
		Task<ResponseDTO?> UpdateCouponAsync(int id,CouponDTO couponDTO);
		Task<ResponseDTO?> DeleteCouponAsync(int id);


	}
}
