using AutoMapper;
using Services.OrderAPI.Models;
using Services.OrderAPI.Models.DTOs;

namespace Services.OrderAPI.Configuration
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<OrderHeaderDTO, CartHeaderDTO>().ForMember(dest => dest.CartTotal, x => x.MapFrom(src => src.OrderTotal)).ReverseMap();
            CreateMap<CartDetailDTO, OrderDetailDTO>().ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name)).ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price)).ReverseMap();
            CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();
            CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
        }
    }
}
