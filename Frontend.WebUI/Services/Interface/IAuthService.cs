using Frontend.WebUI.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Frontend.WebUI.Services.Interface
{
	public interface IAuthService
	{
		Task<ResponseDTO?> Login(LoginRequestDTO login);
		Task<ResponseDTO?> Register(RegistrationRequestDTO registration);
	}
}
