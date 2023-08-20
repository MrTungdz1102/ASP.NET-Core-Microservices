using Frontend.WebUI.Models.DTOs;
using Frontend.WebUI.Services.Interface;
using System.Security.Policy;

namespace Frontend.WebUI.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _service;

        public ProductService(IBaseService service)
        {
            _service = service;
        }
        public async Task<ResponseDTO?> CreateProduct(ProductDTO productDTO)
        {
            return await _service.SendAsync(new RequestDTO
            {
                Url = Utility.Constants.ProductAPIBase + "/api/ProductAPI/CreateProduct",
                Data = productDTO,
                ApiType = Utility.Constants.ApiType.POST
            });
        }

        public async Task<ResponseDTO?> DeleteProduct(int id)
        {
            return await _service.SendAsync(new RequestDTO
            {
                Url = Utility.Constants.ProductAPIBase + "/api/ProductAPI/DeleteProduct/" + id,
                ApiType = Utility.Constants.ApiType.DELETE
            });
        }

        public async Task<ResponseDTO?> GetAllProduct()
        {
            return await _service.SendAsync(new RequestDTO
            {
                Url = Utility.Constants.ProductAPIBase + "/api/ProductAPI/GetAllProduct",
                ApiType = Utility.Constants.ApiType.GET
            });
        }

        public async Task<ResponseDTO?> GetProductById(int id)
        {
            return await _service.SendAsync(new RequestDTO
            {
                Url = Utility.Constants.ProductAPIBase + "/api/ProductAPI/GetProductById/" + id,
                ApiType = Utility.Constants.ApiType.GET
            });
        }

        public async Task<ResponseDTO?> UpdateProduct(int id, ProductDTO productDTO)
        {
            return await _service.SendAsync(new RequestDTO
            {
                Url = Utility.Constants.ProductAPIBase + "/api/ProductAPI/UpdateProduct/" + id,
                ApiType = Utility.Constants.ApiType.PUT,
                Data = productDTO
            });
        }
    }
}
