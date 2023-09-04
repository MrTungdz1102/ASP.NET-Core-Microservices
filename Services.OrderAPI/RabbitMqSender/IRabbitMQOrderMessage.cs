namespace Services.OrderAPI.RabbitMqSender
{
	public interface IRabbitMQOrderMessage
	{
		void SendMessage(object message, string exchangeName);
	}
}
