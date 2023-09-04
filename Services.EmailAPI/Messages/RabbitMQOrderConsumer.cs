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

    public class RabbitMQOrderConsumer : BackgroundService
	{
		// co the trien khai interface start, stop giong voi azure message bus

		private readonly EmailService _emailService;
		private readonly IConfiguration _configuration;
		private IConnection? _connection;
		private IModel _channel;
		private string queueName = "";
		public RabbitMQOrderConsumer(IConfiguration configuration, EmailService emailService)
		{
			_configuration = configuration;
			_emailService = emailService;
			var factory = new ConnectionFactory
			{
				HostName = "localhost",
				UserName = "guest",
				Password = "guest"
			};
			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();
			_channel.ExchangeDeclare(_configuration["TopicAndQueueNames:OrderCreatedTopic"], ExchangeType.Fanout);
			queueName = _channel.QueueDeclare().QueueName;
			_channel.QueueBind(queueName, _configuration["TopicAndQueueNames:OrderCreatedTopic"], "");
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
			_channel.BasicConsume(queueName, false, consumer);
		}

		private async Task HandleMessage(TopicEmailMessage exchangeFanout)
		{
			await _emailService.LogOrderPlaced(exchangeFanout);
		}
	}
}

