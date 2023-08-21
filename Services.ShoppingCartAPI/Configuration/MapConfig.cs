using AutoMapper;
using Services.ShoppingCartAPI.Models;
using Services.ShoppingCartAPI.Models.DTOs;

namespace Services.ShoppingCartAPI.Configuration
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<CartDetail, CartDTO>().ReverseMap();
            CreateMap<CartHeader, CartDTO>().ReverseMap();

            CreateMap<CartDetailDTO, CartDTO>().ReverseMap();
            CreateMap<CartHeaderDTO, CartDTO>().ReverseMap();

            CreateMap<CartDetailDTO, CartDetail>().ReverseMap();
           CreateMap<CartHeaderDTO, CartHeader>().ReverseMap();
        }
    }
}
