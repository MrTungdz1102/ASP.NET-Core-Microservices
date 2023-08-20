using AutoMapper;
using Services.ProductAPI.Models;
using Services.ProductAPI.Models.DTOs;

namespace Services.ProductAPI.Configuration
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
        }
    }
}
