using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Services.EmailAPI.Models;
using Services.EmailAPI.Models.DTOs;
using Services.EmailAPI.Services;
using System.Text;
using System.Threading.Channels;

namespace Services.EmailAPI.Messages
{

    public class RabbitMQOrderConsumerDirect : BackgroundService
	{
		// co the trien khai interface start, stop giong voi azure message bus

		private readonly EmailService _emailService;
		private readonly IConfiguration _configuration;
		private IConnection? _connection;
		private IModel _channel;
		private const string OrderCreated_EmailUpdateQueue = "EmailUpdateQueue";
		private string ExchangeName = "";
		public RabbitMQOrderConsumerDirect(IConfiguration configuration, EmailService emailService)
		{
			_configuration = configuration;
			_emailService = emailService;
			var factory = new ConnectionFactory
			{
				HostName = "localhost",
				UserName = "guest",
				Password = "guest"
			};
			ExchangeName = _configuration["TopicAndQueueNames:OrderCreatedTopic"];
			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();
			_channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
			_channel.QueueDeclare(OrderCreated_EmailUpdateQueue, false, false, false, null);
			_channel.QueueBind(OrderCreated_EmailUpdateQueue, ExchangeName, "EmailUpdate");
		}
		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += async (sender, e) =>
			{
				var content = Encoding.UTF8.GetString(e.Body.ToArray());
				TopicEmailMessage exchangeFanout = JsonConvert.DeserializeObject<TopicEmailMessage>(content);
				await HandleMessage(exchangeFanout);

				_channel.BasicAck(e.DeliveryTag, false);
			};
			_channel.BasicConsume(OrderCreated_EmailUpdateQueue, false, consumer);
		}

		private async Task HandleMessage(TopicEmailMessage exchangeFanout)
		{
			await _emailService.LogOrderPlaced(exchangeFanout);
		}
	}
}

