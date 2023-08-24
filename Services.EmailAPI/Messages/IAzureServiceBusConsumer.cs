namespace Services.EmailAPI.Messages
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
