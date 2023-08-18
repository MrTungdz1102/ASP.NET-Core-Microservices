using AutoMapper;
using Services.AuthAPI.Models;
using Services.AuthAPI.Models.DTOs;

namespace Services.AuthAPI.Configuration
{
	public class MapConfig : Profile
	{
		public MapConfig()
		{
			CreateMap<AppUser, RegistrationRequestDTO>().ReverseMap();
			CreateMap<AppUser, UserDTO>().ReverseMap();
		}
	}
}
