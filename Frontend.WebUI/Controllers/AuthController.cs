using Frontend.WebUI.Models.DTOs;
using Frontend.WebUI.Services.Interface;
using Frontend.WebUI.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Frontend.WebUI.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _auth;
        private readonly ITokenProvider _token;

        public AuthController(IAuthService auth, ITokenProvider token)
		{
			_auth = auth;
			_token = token;
		}
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginRequestDTO login)
		{
			ResponseDTO responseDTO = await _auth.Login(login);
			if (responseDTO.Result is not null && responseDTO.IsSuccess)
			{
				LoginResponseDTO loginResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(responseDTO.Result));
				await SignInUser(loginResponse);
				_token.SetToken(loginResponse.Token);
				return RedirectToAction("Index", "Home");
			}
			else
			{
                // ModelState.AddModelError("LoginError", responseDTO.Message);
                TempData["error"] = responseDTO?.Message;
                return View();
			}
		}

		public IActionResult Register()
		{
			var listRoles = new List<SelectListItem>
			{
				new SelectListItem{ Text = Constants.RoleUser, Value = Constants.RoleUser},
				new SelectListItem{ Text = Constants.RoleAdmin, Value = Constants.RoleAdmin}
			};
			ViewBag.RoleList = listRoles;
            return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegistrationRequestDTO registration)
		{
			var result = await _auth.Register(registration);
			if(result is not null && result.IsSuccess)
			{
				TempData["success"] = "Create account successfully. Please login!";
				return RedirectToAction("Login", "Auth");
			}
			else {
				TempData["error"] = result?.Message;
                var listRoles = new List<SelectListItem>
            {
                new SelectListItem{ Text = Constants.RoleUser, Value = Constants.RoleUser},
                new SelectListItem{ Text = Constants.RoleAdmin, Value = Constants.RoleAdmin}
            };
                ViewBag.RoleList = listRoles;
            }
            return View();
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			_token.ClearToken();
			return RedirectToAction("Index", "Home");
		}

		private async Task SignInUser(LoginResponseDTO login)
		{
			var handler = new JwtSecurityTokenHandler();
			var jwt = handler.ReadJwtToken(login.Token);
			var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
			identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
		}
	}
}
