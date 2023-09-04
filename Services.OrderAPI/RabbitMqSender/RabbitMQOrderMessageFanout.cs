using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Services.OrderAPI.RabbitMqSender
{
	public class RabbitMQOrderMessageFanout : IRabbitMQOrderMessage
	{
		// fanout exchange
		private readonly string _hostName;
		private readonly string _userName;
		private readonly string _passWord;
		private IConnection? _connection;
		public RabbitMQOrderMessageFanout()
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
			
			var json = JsonConvert.SerializeObject(message);
			var body = Encoding.UTF8.GetBytes(json);

			channel.BasicPublish(exchange: exchangeName, "", null, body: body);
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
