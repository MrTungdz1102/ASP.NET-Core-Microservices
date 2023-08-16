using Frontend.WebUI.Models.DTOs;
using Frontend.WebUI.Services.Interface;

namespace Frontend.WebUI.Services.Implementation
{
	public class CouponService : ICouponService
	{
		private readonly IBaseService _baseService;

		public CouponService(IBaseService baseService)
		{
			_baseService = baseService;
		}
		public async Task<ResponseDTO?> CreateCouponAsync(CouponDTO couponDTO)
		{
			return await _baseService.SendAsync(new RequestDTO
			{
				ApiType = Utility.Constants.ApiType.POST,
				Url = Utility.Constants.CouponAPIBase + "/api/CouponAPI/CreateCoupon",
				Data = couponDTO
			});
		}

		public async Task<ResponseDTO?> DeleteCouponAsync(int id)
		{
			return await _baseService.SendAsync(new RequestDTO
			{
				ApiType = Utility.Constants.ApiType.DELETE,
				Url = Utility.Constants.CouponAPIBase + "/api/CouponAPI/DeleteCoupon/" + id
			});
		}

		public async Task<ResponseDTO?> GetAllCouponAsync()
		{
			return await _baseService.SendAsync(new RequestDTO
			{
				ApiType = Utility.Constants.ApiType.GET,
				Url = Utility.Constants.CouponAPIBase + "/api/CouponAPI/GetAllCoupon"
			});
		}

		public async Task<ResponseDTO?> GetCouponAsync(string code)
		{
			return await _baseService.SendAsync(new RequestDTO
			{
				ApiType = Utility.Constants.ApiType.GET,
				Url = Utility.Constants.CouponAPIBase + "/api/CouponAPI/GetByCode/" + code
			});
		}

		public async Task<ResponseDTO?> GetCouponByIdAsync(int id)
		{
			return await _baseService.SendAsync(new RequestDTO
			{
				ApiType = Utility.Constants.ApiType.GET,
				Url = Utility.Constants.CouponAPIBase + "/api/CouponAPI/GetCouponById/" + id
			});
		}

		public async Task<ResponseDTO?> UpdateCouponAsync(int id, CouponDTO couponDTO)
		{
			return await _baseService.SendAsync(new RequestDTO
			{
				ApiType = Utility.Constants.ApiType.PUT,
				Url = Utility.Constants.CouponAPIBase + "/api/CouponAPI/UpdateCoupon/" + id,
				Data = couponDTO
			});
		}
	}
}
