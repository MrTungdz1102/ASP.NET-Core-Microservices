using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.AuthAPI.Models.DTOs;
using Services.AuthAPI.Services.Interface;

namespace Services.AuthAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthAPIController : ControllerBase
	{
		private readonly IAuthService _auth;
		private ResponseDTO _response;
		public AuthAPIController(IAuthService auth)
		{
			_auth = auth;
			_response = new ResponseDTO();
		}
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registration)
		{
			var errorMessage = await _auth.Register(registration);
			if(errorMessage.Any())
			{
				_response.IsSuccess = false;
				foreach (var item in errorMessage)
				{
					_response.Message += item.Code +" : "+ item.Description + " , ";
				}
				return BadRequest(_response);
			}
			else
			{
				_response.Message = "Thanh cong";
			}
			return Ok(_response);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO login)
		{
			var result = await _auth.Login(login);
			if (result.User == null)
			{
				_response.IsSuccess = false;
				_response.Message = "Username or password is incorrect";
				return BadRequest(_response);
			}
			_response.Result = result;
			return Ok(_response);
		}
	}
}
