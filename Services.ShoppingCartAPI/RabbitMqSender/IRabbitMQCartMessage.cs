namespace Services.ShoppingCartAPI.RabbitMqSender
{
	public interface IRabbitMQCartMessage
	{
		void SendMessage(object message, string queueName);
	}
}
