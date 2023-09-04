using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Services.TopicSubscriptionAPI.Models;
using Services.TopicSubscriptionAPI.Services;
using System.Text;

namespace Services.TopicSubscriptionAPI.Messages
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string? serviceBusConnectionString;
        private readonly string? orderCreatedTopic;
        private readonly string orderCreatedSubscription;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _topicProcessor;
        // do chi dang ky builder.Services.AddSingleton(new EmailService(optionBuilder.Options));
        // nen khong su dung interface ma thay vao do su dung class
        private readonly TopicService _topicService;
        public AzureServiceBusConsumer(IConfiguration configuration, TopicService topicService)
        {
            _configuration = configuration;
            _topicService = topicService;
            serviceBusConnectionString = _configuration["ServiceBusConnectionString"];
            // _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue")
            orderCreatedTopic = _configuration["TopicAndQueueNames:OrderCreatedTopic"];
            orderCreatedSubscription = _configuration["TopicAndQueueNames:OrderCreated_Subscription"];

            var client = new ServiceBusClient(serviceBusConnectionString);
            _topicProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedSubscription); 
        }

        public async Task Start()
        {
            _topicProcessor.ProcessMessageAsync += OnNewOrderRewardRequestReceived; // reward topic
            _topicProcessor.ProcessErrorAsync += ErrorHandler;
            await _topicProcessor.StopProcessingAsync();

        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnNewOrderRewardRequestReceived(ProcessMessageEventArgs arg)
        {
            // receive message
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            TopicRewardMessage topicRewardMessage = JsonConvert.DeserializeObject<TopicRewardMessage>(body);
            try
            {
                // log email
                await _topicService.UpdateRewardTopic(topicRewardMessage);
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Stop()
        {
            await _topicProcessor.StopProcessingAsync();
            await _topicProcessor.DisposeAsync();
        }
    }
}
