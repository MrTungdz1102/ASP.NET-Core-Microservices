using AutoMapper;
using Services.CouponAPI.Models;
using Services.CouponAPI.Models.DTOs;

namespace Services.CouponAPI.Configurations
{
	public class MapConfig : Profile
	{
		public MapConfig()
		{
			CreateMap<CouponDTO, Coupon>().ReverseMap();
			
		}
	}
}
