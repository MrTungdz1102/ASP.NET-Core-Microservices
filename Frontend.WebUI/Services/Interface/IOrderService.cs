using Frontend.WebUI.Models.DTOs;

namespace Frontend.WebUI.Services.Interface
{
    public interface IOrderService
    {
        Task<ResponseDTO?> CreateOrder(CartDTO cartDTO);
        Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequestDTO);
    }
}
