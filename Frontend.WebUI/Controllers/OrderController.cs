using Frontend.WebUI.Models.DTOs;
using Frontend.WebUI.Services.Interface;
using Frontend.WebUI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Frontend.WebUI.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            return View();
        }

		public async Task<IActionResult> Detail(int orderId)
		{
            var response = await _orderService.GetOrderById(orderId);
            OrderHeaderDTO result = null;
			string userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
			if (response is not null && response.IsSuccess)
            {
                result = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
            }
            if(!User.IsInRole(Utility.Constants.RoleAdmin) && userId != result.UserId)
            {
                return NotFound();
            }
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> OrderReadyToPickUp(int orderId)
        {
            OrderHeaderDTO result = null;
            string userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            var response = await _orderService.UpdateOrderStatus(orderId, Constants.Status_ReadyForPickup);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Order Status Updated Successfully";
                return RedirectToAction("Detail", orderId);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            OrderHeaderDTO result = null;
            string userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            var response = await _orderService.UpdateOrderStatus(orderId, Constants.Status_Completed);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Order Status Updated Successfully";
                return RedirectToAction("Detail", orderId);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            OrderHeaderDTO result = null;
            string userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            var response = await _orderService.UpdateOrderStatus(orderId, Constants.Status_Cancelled);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Order Status Cancel Successfully";
                return RedirectToAction("Detail", orderId);
            }
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            string userId = "";
            IEnumerable<OrderHeaderDTO>? orderList;
            if (!User.IsInRole(Utility.Constants.RoleAdmin))
            {
                userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            }
			ResponseDTO? response = await _orderService.GetAllOrder(userId);
			if (response != null && response.IsSuccess)
            {
                orderList = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDTO>>(Convert.ToString(response.Result));
                switch (status)
                {
                    case "approved":
                        orderList = orderList.Where(x => x.Status == Constants.Status_Approved);
                        break;
                    case "readytopickup":
                        orderList = orderList.Where(x => x.Status == Constants.Status_ReadyForPickup);
                        break;
                    case "cancelled":
                        orderList = orderList.Where(x => x.Status == Constants.Status_Cancelled);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                TempData["error"] = response.Message;
                orderList = new List<OrderHeaderDTO>();
            }
            return Json(new { data = orderList });
        }
    }
}
