using Shared.Messages;

namespace Shared.Events
{
    public class OrderCreatedEvent
	{
		public int BuyerId { get; set; }
		public Decimal TotalPrice { get; set; }
		public List<OrderItemMessage> OrderItems { get; set; }
		public Guid IdempotentToken { get; set; }
	}
}

