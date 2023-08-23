using Frontend.WebUI.Models.DTOs;
using Frontend.WebUI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Frontend.WebUI.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _cartService;

        public ShoppingCartController(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await LoadCartOnLoggedUser());
        }

        public async Task<IActionResult> RemoveProduct(int id)
        {          
            ResponseDTO? response = await _cartService.RemoveCart(id);
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Remove product successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDTO)
        {
            ResponseDTO? response = await _cartService.ApplyCoupon(cartDTO);
            if (response.Result is not null)
            {
                TempData["success"] = "Apply coupon successfully";
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDTO cartDTO)
        {
            cartDTO.CartHeader.CouponCode = string.Empty;
            ResponseDTO? response = await _cartService.ApplyCoupon(cartDTO);
            if (response.Result is not null)
            {
                TempData["success"] = "Remove coupon successfully";
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<CartDTO> LoadCartOnLoggedUser()
        {
              var userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            //var claimIdentity = (ClaimsIdentity)User.Identity;
            //var userId = claimIdentity.FindFirst(JwtRegisteredClaimNames.Sub).Value;
            ResponseDTO? response = await _cartService.GetCartByUserId(userId);
            if(response is not null && response.IsSuccess)
            {
                CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                return cartDTO;
            }
            return new CartDTO();
        }
    }
}
