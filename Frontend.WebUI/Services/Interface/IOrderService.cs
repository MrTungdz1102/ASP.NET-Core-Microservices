using Frontend.WebUI.Models.DTOs;

namespace Frontend.WebUI.Services.Interface
{
    public interface IOrderService
    {
        Task<ResponseDTO?> CreateOrder(CartDTO cartDTO);
        Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequestDTO);
        Task<ResponseDTO?> ValidateStripeSession(int orderHeaderId);
        Task<ResponseDTO?> GetAllOrder(string? userId);
        Task<ResponseDTO?> GetOrderById(int id);
        Task<ResponseDTO?> UpdateOrderStatus(int orderId, string newStatus);
    }
}
