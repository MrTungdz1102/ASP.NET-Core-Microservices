using Frontend.WebUI.Models.DTOs;
using Frontend.WebUI.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Frontend.WebUI.Controllers
{
	public class CouponController : Controller
	{
		private readonly ICouponService _couponService;

		public CouponController(ICouponService couponService)
		{
			_couponService = couponService;
		}
		public async Task<IActionResult> Index()
		{
			List<CouponDTO>? coupons = new List<CouponDTO>();
			ResponseDTO? response = await _couponService.GetAllCouponAsync();
			if(response is not null && response.IsSuccess) {
				coupons = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response.Result));
			}
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(coupons);
		}

		public IActionResult CreateCoupon()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateCoupon(CouponDTO couponDTO)
		{
			ResponseDTO? response = await _couponService.CreateCouponAsync(couponDTO);
			if (response is not null && response.IsSuccess)
			{
				TempData["success"] = "Coupon created successfully";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["error"] = response?.Message;
			}
			return View(couponDTO);
		}

		public async Task<IActionResult> UpdateCoupon(int id)
		{
			CouponDTO? coupon = null;
			ResponseDTO ? response = await _couponService.GetCouponByIdAsync(id);
			if (response is not null && response.IsSuccess)
			{
				coupon = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
			}
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(coupon);
		}

		[HttpPost]
        public async Task<IActionResult> UpdateCoupon(int id, CouponDTO couponDTO)
        {
            ResponseDTO? response = await _couponService.UpdateCouponAsync(id, couponDTO);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Coupon updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(couponDTO);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteCoupon(int couponId)
        {           		
            ResponseDTO? response = await _couponService.DeleteCouponAsync(couponId);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Coupon deleted successfully";
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
