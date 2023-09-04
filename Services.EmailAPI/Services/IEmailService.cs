using Services.EmailAPI.Models;
using Services.EmailAPI.Models.DTOs;

namespace Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDTO cartDto);
        Task RegisterUserEmailAndLog(string email);
        Task LogOrderPlaced(TopicEmailMessage topicEmailMessage);
    }
}
