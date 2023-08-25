using Services.OrderAPI.Models.DTOs;

namespace Services.OrderAPI.Service.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProduct();
    }
}
