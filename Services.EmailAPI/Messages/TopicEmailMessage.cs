namespace Services.EmailAPI.Messages
{
    public class TopicEmailMessage
    {
        public string UserId { get; set; }
        public int TopicActivity { get; set; }
        public int OrderId { get; set; }
    }
}
