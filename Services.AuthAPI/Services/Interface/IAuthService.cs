using Microsoft.AspNetCore.Identity;
using Services.AuthAPI.Models.DTOs;

namespace Services.AuthAPI.Services.Interface
{
	public interface IAuthService
	{
		Task<IEnumerable<IdentityError>> Register(RegistrationRequestDTO registration);
		Task<LoginResponseDTO> Login(LoginRequestDTO login);
		Task<bool> AssignRole(string email, string roleName);
	}
}
