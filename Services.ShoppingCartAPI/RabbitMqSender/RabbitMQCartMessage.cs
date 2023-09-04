using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Services.ShoppingCartAPI.RabbitMqSender
{
	public class RabbitMQCartMessage : IRabbitMQCartMessage
	{
		private readonly string _hostName;
		private readonly string _userName;
		private readonly string _passWord;
		private IConnection? _connection;
		public RabbitMQCartMessage()
		{
			_hostName = "localhost";
			_userName = "guest";
			_passWord = "guest";
		}
		public void SendMessage(object message, string queueName)
		{
			if (!ConnectionExits())
			{
				CreateConnection();
			}
			using var channel = _connection.CreateModel();
			channel.QueueDeclare(queueName, false, false, false, null);

			var json = JsonConvert.SerializeObject(message);
			var body = Encoding.UTF8.GetBytes(json);

			channel.BasicPublish(exchange: "", routingKey: queueName, null, body: body);
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
