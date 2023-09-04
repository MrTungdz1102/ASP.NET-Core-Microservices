using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Services.OrderAPI.RabbitMqSender
{
	public class RabbitMQOrderMessageDirect : IRabbitMQOrderMessage
	{
		// direct exchange
		private readonly string _hostName;
		private readonly string _userName;
		private readonly string _passWord;
		private IConnection? _connection;
		private const string OrderCreated_RewardsUpdateQueue = "RewardsUpdateQueue";
		private const string OrderCreated_EmailUpdateQueue = "EmailUpdateQueue";
		public RabbitMQOrderMessageDirect()
		{
			_hostName = "localhost";
			_userName = "guest";
			_passWord = "guest";
		}
		public void SendMessage(object message, string exchangeName)
		{
			if (!ConnectionExits())
			{
				CreateConnection();
			}
			using var channel = _connection.CreateModel();
			// durable = true thi khi ung dung restart, exchange van ton tai
			channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: false);

			channel.QueueDeclare(OrderCreated_EmailUpdateQueue, false, false, false, null);
			channel.QueueDeclare(OrderCreated_RewardsUpdateQueue, false, false, false, null);

			channel.QueueBind(OrderCreated_EmailUpdateQueue, exchangeName, "EmailUpdate");
			channel.QueueBind(OrderCreated_RewardsUpdateQueue, exchangeName, "RewardsUpdate");

			var json = JsonConvert.SerializeObject(message);
			var body = Encoding.UTF8.GetBytes(json);

			channel.BasicPublish(exchange: exchangeName, "EmailUpdate", null, body: body);
			channel.BasicPublish(exchange: exchangeName, "RewardsUpdate", null, body: body);
		}

		private void CreateConnection()
		{
			try
			{
				var factory = new ConnectionFactory
				{
					HostName = _hostName,
					UserName = _userName,
					Password = _passWord
				};
				_connection = factory.CreateConnection();
			}
			catch (Exception ex)
			{

			}
		}

		private bool ConnectionExits()
		{
			return _connection == null ? false : true;
		}
	}
}
