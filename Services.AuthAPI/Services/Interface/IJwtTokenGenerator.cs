using Services.AuthAPI.Models;

namespace Services.AuthAPI.Services.Interface
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(AppUser appUser);
	}
}
