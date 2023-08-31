using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Services.TopicSubscriptionAPI.Data;
using Services.TopicSubscriptionAPI.Messages;
using Services.TopicSubscriptionAPI.Models;

namespace Services.TopicSubscriptionAPI.Services
{
    public class TopicService : ITopicService
    {
        private DbContextOptions<AppDbContext> _dbContextOptions;
        public TopicService(DbContextOptions<AppDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task UpdateRewardTopic(TopicRewardMessage message)
        {
            try
            {
                Topic topic = new()
                {
                   OrderId = message.OrderId,
                   UserId = message.UserId,
                   TopicActivity = message.TopicActivity,
                   RewardsDate = DateTime.Now
                };
                await using var _db = new AppDbContext(_dbContextOptions);
                await _db.Topics.AddAsync(topic);
                await _db.SaveChangesAsync(); 
            }
            catch (Exception ex)
            {
               
            }
        }
    }
}
