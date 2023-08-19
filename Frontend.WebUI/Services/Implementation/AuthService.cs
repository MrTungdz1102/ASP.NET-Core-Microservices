using Frontend.WebUI.Models.DTOs;
using Frontend.WebUI.Services.Interface;
using NuGet.Protocol.Plugins;

namespace Frontend.WebUI.Services.Implementation
{
	public class AuthService : IAuthService
	{
		private readonly IBaseService _baseService;
		public AuthService(IBaseService baseService)
		{
			_baseService = baseService;
		}
		public async Task<ResponseDTO?> Login(LoginRequestDTO login)
		{
			return await _baseService.SendAsync(new RequestDTO
			{
				ApiType = Utility.Constants.ApiType.POST,
				Url = Utility.Constants.AuthAPIBase + "/api/AuthAPI/login",
				Data = login
			}, withBearer: false);
		}

		public async Task<ResponseDTO?> Register(RegistrationRequestDTO registration)
		{
			return await _baseService.SendAsync(new RequestDTO
			{
				ApiType = Utility.Constants.ApiType.POST,
				Url = Utility.Constants.AuthAPIBase + "/api/AuthAPI/register",
				Data = registration
			}, withBearer: false);
		}
	}
}
