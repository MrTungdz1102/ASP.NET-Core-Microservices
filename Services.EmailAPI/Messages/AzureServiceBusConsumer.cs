using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Services.EmailAPI.Models.DTOs;
using Services.EmailAPI.Services;
using System.Text;

namespace Services.EmailAPI.Messages
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string? serviceBusConnectionString;
        private readonly string? emailCartQueue;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _emailCartProcessor;
        // do chi dang ky builder.Services.AddSingleton(new EmailService(optionBuilder.Options));
        // nen khong su dung interface ma thay vao do su dung class
        private readonly EmailService _emailService;
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            serviceBusConnectionString = _configuration["ServiceBusConnectionString"];
            // _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue")
            emailCartQueue = _configuration["TopicAndQueueNames:EmailShoppingCartQueue"];

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;

        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs arg)
        {
            // receive message
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(body);
            try
            {
                // log email
                await _emailService.EmailCartAndLog(cartDTO);
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
        }
    }
}
