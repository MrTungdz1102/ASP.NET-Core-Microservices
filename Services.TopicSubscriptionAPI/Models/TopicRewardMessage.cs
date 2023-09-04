namespace Services.TopicSubscriptionAPI.Models
{
    public class TopicRewardMessage
    {
        public string UserId { get; set; }
        public int TopicActivity { get; set; }
        public int OrderId { get; set; }
    }
}
