using Services.TopicSubscriptionAPI.Messages;

namespace Services.TopicSubscriptionAPI.Services
{
    public interface ITopicService
    {
        Task UpdateRewardTopic(TopicRewardMessage message);
    }
}
