using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Services.TopicSubscriptionAPI.Models;
using Services.TopicSubscriptionAPI.Services;
using System.Text;
using System.Threading.Channels;

namespace Services.TopicSubscriptionAPI.Messages
{

	public class RabbitMQOrderConsumerDirect : BackgroundService
	{
		// co the trien khai interface start, stop giong voi azure message bus

		private readonly IConfiguration _configuration;
		private IConnection? _connection;
		private IModel _channel;
		private readonly TopicService _topicService;
		private string OrderCreated_RewardsUpdateQueue = "RewardsUpdateQueue";
		private string ExchangeName = "";
		public RabbitMQOrderConsumerDirect(IConfiguration configuration, TopicService topicService)
		{
			_configuration = configuration;
			_topicService = topicService;
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
			_channel.QueueDeclare(OrderCreated_RewardsUpdateQueue, false, false, false, null);
			_channel.QueueBind(OrderCreated_RewardsUpdateQueue, ExchangeName, "RewardsUpdate");
		}
		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += async (sender, e) =>
			{
				var content = Encoding.UTF8.GetString(e.Body.ToArray());
				TopicRewardMessage exchangeFanout = JsonConvert.DeserializeObject<TopicRewardMessage>(content);
				await HandleMessage(exchangeFanout);

				_channel.BasicAck(e.DeliveryTag, false);
			};
			_channel.BasicConsume(OrderCreated_RewardsUpdateQueue, false, consumer);
		}

		private async Task HandleMessage(TopicRewardMessage exchangeFanout)
		{
			await _topicService.UpdateRewardTopic(exchangeFanout);
		}
	}
}

