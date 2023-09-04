using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Services.EmailAPI.Services;
using System.Text;
using System.Threading.Channels;

namespace Services.EmailAPI.Messages
{

	public class RabbitMQAuthConsumer : BackgroundService
	{
		// co the trien khai interface start, stop giong voi azure message bus

		private readonly EmailService _emailService;
		private readonly IConfiguration _configuration;
		private IConnection? _connection;
		private IModel _channel;
		public RabbitMQAuthConsumer(IConfiguration configuration, EmailService emailService)
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
			_channel.QueueDeclare(_configuration["TopicAndQueueNames:RegisterUserEmailQueue"], false, false, false, null);
		}
		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += async (sender, e) =>
			{
				var content = Encoding.UTF8.GetString(e.Body.ToArray());
				string email = JsonConvert.DeserializeObject<string>(content);
				await HandleMessage(email);

				_channel.BasicAck(e.DeliveryTag, false);
			};
			_channel.BasicConsume(_configuration["TopicAndQueueNames:RegisterUserEmailQueue"], false, consumer);
		}

		private async Task HandleMessage(string email)
		{
			await _emailService.RegisterUserEmailAndLog(email);
		}
	}
}

