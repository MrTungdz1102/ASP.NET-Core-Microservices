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
        private readonly string registerUserQueue;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _registerUserQueue;
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
            registerUserQueue = _configuration["TopicAndQueueNames:RegisterUserEmailQueue"];

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);

            _registerUserQueue = client.CreateProcessor(registerUserQueue);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StopProcessingAsync();

            _registerUserQueue.ProcessMessageAsync += OnUserRegisterRequestReceived;
            _registerUserQueue.ProcessErrorAsync += ErrorHandler;
            await _registerUserQueue.StopProcessingAsync();

        }

        private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body);
            try
            {
                //TODO - try to log email
                await _emailService.RegisterUserEmailAndLog(email);
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
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

            await _registerUserQueue.StopProcessingAsync();
            await _registerUserQueue.DisposeAsync();
        }
    }
}
