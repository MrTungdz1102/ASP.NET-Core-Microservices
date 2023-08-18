using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Services.AuthAPI.Models;
using Services.AuthAPI.Models.DTOs;
using Services.AuthAPI.Services.Interface;

namespace Services.AuthAPI.Services.Implementation
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<AppUser> _userManager;
		private AppUser? _user;
		private readonly IMapper _mapper;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IJwtTokenGenerator _jwtToken;
		public AuthService(UserManager<AppUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtToken)
		{
			_userManager = userManager;
			_mapper = mapper;
			_roleManager = roleManager;
			_jwtToken = jwtToken;
		}
		public async Task<LoginResponseDTO> Login(LoginRequestDTO login)
		{
			bool checkPass = false;
			_user = await _userManager.FindByEmailAsync(login.UserName);
			if (_user != null)
			{
				checkPass = await _userManager.CheckPasswordAsync(_user, login.Password);
			}
			if (_user == null || checkPass == false)
			{
				return new LoginResponseDTO() { User = null, Token = "" };
			}
			else
			{
				var token = _jwtToken.GenerateToken(_user);
				return new LoginResponseDTO { User = _mapper.Map<UserDTO>(_user), Token = token };
			}
		}

		public async Task<IEnumerable<IdentityError>> Register(RegistrationRequestDTO registration)
		{
			_user = _mapper.Map<AppUser>(registration);
			_user.UserName = registration.Email;
			_user.Name = registration.Name;
			var result = await _userManager.CreateAsync(_user, registration.Password);
			if (result.Succeeded)
			{
				if (string.IsNullOrEmpty(registration.Role))
				{
					await AssignRole(registration.Email, "USER");
				}
				else
				{
					await AssignRole(registration.Email, registration.Role);
				}
			}
			return result.Errors;
		}

		public async Task<bool> AssignRole(string email, string roleName)
		{
			_user = await _userManager.FindByEmailAsync(email);
			if (_user != null)
			{
				if (!await _roleManager.RoleExistsAsync(roleName))
				{
					await _roleManager.CreateAsync(new IdentityRole(roleName));
				}
				await _userManager.AddToRoleAsync(_user, roleName);
				return true;
			}
			return false;
		}
	}
}
