namespace Services.AuthAPI.RabbitMqSender
{
	public interface IRabbitMQAuthMessage
	{
		void SendMessage(object message, string queueName);
	}
}
