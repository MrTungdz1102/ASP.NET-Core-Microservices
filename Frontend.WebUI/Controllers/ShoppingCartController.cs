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
        private readonly IOrderService _orderService;

        public ShoppingCartController(IShoppingCartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await LoadCartOnLoggedUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartOnLoggedUser());
        }

        [Authorize]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            ResponseDTO? response = await _orderService.ValidateStripeSession(orderId);
            if (response is not null && response.IsSuccess)
            {
                OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
                if(orderHeaderDTO.Status == Utility.Constants.Status_Approved)
                {
                    return View(orderId);
                }
            }
            return View(orderId);
        }



        [HttpPost]
        public async Task<IActionResult> Checkout(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartOnLoggedUser();
            cart.CartHeader.Phone = cartDTO.CartHeader.Phone;
            cart.CartHeader.Email = cartDTO.CartHeader.Email;
            cart.CartHeader.Name = cartDTO.CartHeader.Name;

            var response = await _orderService.CreateOrder(cart);
            OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
            if(response.IsSuccess && response is not null)
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                StripeRequestDTO stripe = new StripeRequestDTO
                {
                    ApprovedUrl = domain + "ShoppingCart/ConfirmOrder?orderId=" + orderHeaderDTO.OrderHeaderId,
                    CancelUrl = domain + "ShoppingCart/Checkout",
                    OrderHeader = orderHeaderDTO,
                };
                var stripeResponse = await _orderService.CreateStripeSession(stripe);
                StripeRequestDTO result = JsonConvert.DeserializeObject<StripeRequestDTO>(Convert.ToString(stripeResponse.Result));
                Response.Headers.Add("Location", result.StripeSessionUrl);
                return new StatusCodeResult(303);
            }
            return View(cartDTO);
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

        // message bus
        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cartDto)
        {
            CartDTO cart = await LoadCartOnLoggedUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _cartService.EmailCart(cart);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
