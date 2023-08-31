namespace Services.TopicSubscriptionAPI.Models
{
    // azure topics and subscriptions
    public class Topic
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime RewardsDate { get; set; }
        public int TopicActivity { get; set; }
        public int OrderId { get; set; }
    }
}
