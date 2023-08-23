using Newtonsoft.Json;
using Services.ShoppingCartAPI.Models.DTOs;
using Services.ShoppingCartAPI.Service.Interface;

namespace Services.ShoppingCartAPI.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _http;

        public ProductService(IHttpClientFactory http)
        {
            _http = http;
        }
        public async Task<IEnumerable<ProductDTO>> GetAllProduct()
        {
            var client = _http.CreateClient("Product");
            var response = await client.GetAsync("api/ProductAPI/GetAllProduct");
            var apiContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if(result is not null && result.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(Convert.ToString(result.Result));
            }
            return new List<ProductDTO>();
        }
    }
}
