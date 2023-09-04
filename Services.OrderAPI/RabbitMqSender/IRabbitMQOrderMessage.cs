namespace Services.OrderAPI.RabbitMqSender
{
	public interface IRabbitMQCartMessage
	{
		void SendMessage(object message, string exchangeName);
	}
}
