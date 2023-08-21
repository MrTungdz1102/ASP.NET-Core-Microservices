using Services.ShoppingCartAPI.Models.DTOs;

namespace Services.ShoppingCartAPI.Service.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProduct();
    }
}
