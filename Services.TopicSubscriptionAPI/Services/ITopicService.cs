using Services.TopicSubscriptionAPI.Models;

namespace Services.TopicSubscriptionAPI.Services
{
    public interface ITopicService
    {
        Task UpdateRewardTopic(TopicRewardMessage message);
    }
}
