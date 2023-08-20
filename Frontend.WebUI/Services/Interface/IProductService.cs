using Frontend.WebUI.Models.DTOs;

namespace Frontend.WebUI.Services.Interface
{
    public interface IProductService
    {
        Task<ResponseDTO?> GetAllProduct();
        Task<ResponseDTO?> GetProductById(int id);
        Task<ResponseDTO?> CreateProduct(ProductDTO productDTO);
        Task<ResponseDTO?> UpdateProduct(int id, ProductDTO productDTO);
        Task<ResponseDTO?> DeleteProduct(int id);
    }
}
