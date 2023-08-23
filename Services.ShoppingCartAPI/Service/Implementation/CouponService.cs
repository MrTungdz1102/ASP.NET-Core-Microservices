using Newtonsoft.Json;
using Services.ShoppingCartAPI.Models.DTOs;
using Services.ShoppingCartAPI.Service.Interface;

namespace Services.ShoppingCartAPI.Service.Implementation
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _http;

        public CouponService(IHttpClientFactory http)
        {
            _http = http;
        }
        public async Task<CouponDTO> GetCoupon(string couponCode)
        {
            HttpClient client = _http.CreateClient("Coupon");
            HttpResponseMessage response = await client.GetAsync("/api/CouponAPI/GetByCode/" + couponCode);
            var apiContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if( result is not null && result.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(result.Result));
            }
            return new CouponDTO();
        }
    }
}
