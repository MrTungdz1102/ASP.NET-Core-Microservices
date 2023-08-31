namespace Services.TopicSubscriptionAPI.Messages
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
